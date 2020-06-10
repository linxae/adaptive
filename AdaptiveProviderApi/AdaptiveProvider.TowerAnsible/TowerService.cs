using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using System.Web;
using System.Security;

namespace AdaptiveProvider.TowerAnsible
{
    public class TowerService : IDisposable
    {
        private static readonly string ApiVersion = "/api/v2/";
        //private static readonly string TokenResource = $"{ApiVersion}tokens/";

        private HttpClientHandler _httpClientHandler;
        private HttpClient _httpClient;
        private bool _disposedValue;
        private TowerApiMap _apiMap;
        private readonly Uri _towerUri;
        private NetworkCredential _credential;
        private AuthenticationMethod _authenticationMethod;
        
        public TowerService(string cs) : this(cs.Url(), cs.AuthenticationMethod(), cs.Credential(), cs.SkipCertificateCheck())
        {
        }

        public TowerService(string towerUrl, NetworkCredential credential, bool skipCertificateValidation) : this(towerUrl, AuthenticationMethod.Basic, credential, skipCertificateValidation)
        {
        }

        public TowerService(string towerUrl, NetworkCredential credential) : this(towerUrl, AuthenticationMethod.Basic, credential, false)
        {
        }

        public TowerService(string towerUrl, string token, bool skipCertificateValidation) : this(towerUrl, AuthenticationMethod.OAuth, new NetworkCredential("", token), skipCertificateValidation)
        {
        }

        public TowerService(string towerUrl, string token) : this(towerUrl, AuthenticationMethod.OAuth, new NetworkCredential("", token), false)
        {
        }

        public TowerService(string towerUrl, AuthenticationMethod authenticationMethod, NetworkCredential credential, bool skipCertificateValidation)
        {
            if (string.IsNullOrWhiteSpace(towerUrl))
            {
                throw new ArgumentException($"Tower API url must be a valid HTTPS url. Provided value is: '{towerUrl}'");
            }

            _towerUri = new Uri(towerUrl.EndsWith("/") ? towerUrl : towerUrl + "/");
            _authenticationMethod = authenticationMethod;

            if (_authenticationMethod == AuthenticationMethod.Basic)
            {
                if (credential == null || string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password)) 
                {
                    throw new ArgumentNullException("User name and password are required for basic authentication");
                }
                
                var userName = HttpUtility.UrlEncode(credential.UserName);
                var password = HttpUtility.UrlEncode(credential.Password);
                var credentialPair = Encoding.UTF8.GetBytes($"{userName}:{password}");

                _credential = new NetworkCredential(string.Empty, Convert.ToBase64String(credentialPair));
            }
            else if (_authenticationMethod == AuthenticationMethod.OAuth)
            {
                if (credential == null || string.IsNullOrEmpty(credential.Password))
                {
                    throw new ArgumentNullException("A token is required for Auth0 authentication");
                }

                _credential = credential;
            }

            if (skipCertificateValidation)
            {
                _httpClientHandler = new HttpClientHandler();
                _httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                _httpClient = new HttpClient(_httpClientHandler);
            }
            else
            {
                _httpClient = new HttpClient();
            }

        }

        private AuthenticationHeaderValue GetAuthenticationHeader()
        {
            if (_authenticationMethod == AuthenticationMethod.Basic)
            {
                return new AuthenticationHeaderValue("Basic", _credential.Password);
            }
            else
            {
                return new AuthenticationHeaderValue("Bearer", _credential.Password);
            }
        }

        private async Task<O> Post<I, O>(string resource, I payload)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(_towerUri, resource));

            request.Headers.Authorization = GetAuthenticationHeader();

            request.Content = new StringContent(JsonSerializer.Serialize<I>(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.Created:
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<O>(data);

                //process other retun codes 400, 403

                default:
                    throw new Exception($"Request '{resource}' failed with status code {response.StatusCode}");
            }
        }

        private async Task<O> Get<O>(string resource)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(_towerUri, resource));

            request.Headers.Authorization = GetAuthenticationHeader();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    var data = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<O>(data);

                default:
                    throw new Exception($"Request '{resource}' failed with status code {response.StatusCode}");
            }
        }

        public async Task<TowerApiMap> EnsureAuthenticated()
        {
            if (_apiMap == null)
            {
                _apiMap = await Get<TowerApiMap>(ApiVersion);
            }

            return _apiMap;
        }

        public async Task<Job> LaunchJobTemplateAsync(int templateId)
        {
            await EnsureAuthenticated();

            JobLaunch launch = new JobLaunch();
            var job = await Post<JobLaunch, Job>($"{_apiMap.Job_templates}{templateId}/launch/", launch);

            while (job.Status == "waiting" || job.Status == "pending" || job.Status == "running")
            {
                await Task.Delay(250);
                job = await Get<Job>(job.JobUrl);
                Console.WriteLine($"Job {job.Name} status is {job.Status}");
            }

            if (job.Failed || job.Status != "successful")
            {
                throw new Exception($"Job {job.JobId} didn't complete successfully: {job.Status}");
            }

            return job;
        }

        //public Job LaunchJobTemplateAsync(int templateId)
        //{
        //    EnsureAuthenticated().Wait();

        //    JobLaunch launch = new JobLaunch();
        //    var job = Post<JobLaunch, Job>($"{_apiMap.Job_templates}{templateId}/launch/", launch).Result;

        //    while (job.Status == "waiting" || job.Status == "pending" || job.Status == "running")
        //    {
        //        Task.Delay(250).Wait();
        //        job = Get<Job>(job.JobUrl).Result;
        //        Console.WriteLine($"Job {job.Name} status is {job.Status}");
        //    }

        //    if (job.Failed || job.Status != "successful")
        //    {
        //        throw new Exception($"Job {job.JobId} didn't complete successfully: {job.Status}");
        //    }

        //    return job;
        //}

        public Job LaunchJobTemplate(int templateId)
        {
            try
            {
                var job = LaunchJobTemplateAsync(templateId).Result;
                return job;
            }
            catch (AggregateException ex)
            {
                throw ex.InnerExceptions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                    _httpClientHandler.Dispose();
                }

                _httpClient = null;
                _httpClientHandler = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
