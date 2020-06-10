$url = "https://ec2-18-222-250-5.us-east-2.compute.amazonaws.com"

$user = "admin" 
$pwd = 'azerty*123' | ConvertTo-SecureString -asPlainText -Force

$credential = New-Object System.Management.Automation.PSCredential($user, $pwd)

<#
add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) {
        return true;
    }
}
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy#>
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12;

$r = Invoke-WebRequest -Uri "$url/api/v2/tokens/" -Credential $credential -Method Post

#curl -user "admin:azerty*123" POST "https://ec2-18-222-250-5.us-east-2.compute.amazonaws.com/api/v2/tokens/"

<#curl -k -X POST \
  -H “Content-Type: application/json”
  -H “Authorization: Bearer <oauth2-token-value>” \
  https://<tower-host>/api/v2/hosts/ #>