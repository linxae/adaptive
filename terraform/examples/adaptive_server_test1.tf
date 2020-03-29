resource "adaptive_server" "my-server" {
    dns = "myserver.example.com"
}

provider "adaptive" {
  endpoint = "https://localhost:5001/privatecloud/provider"
}
