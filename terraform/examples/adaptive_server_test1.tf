
variable "app_id" {
  description = "Unique application id, it will be used to generate servers/vip dns records"
  type = string
  default = "APP1"
}

variable "web_zone" {
  description = "Network zone where the web servers are deployed"
  type = string
  default = "DMZ1"
}

variable "bundle" {
  description = "Network zone where the web servers are deployed"
  type = string
  default = "B1"
}

variable "server_count" {
  description = "Network zone where the web servers are deployed"
  type = number
  default = 1
}

resource "adaptive_server" "web-server1" {
  count = var.server_count
  app = var.app_id
  bundle = var.bundle
  domain = "testdmz.example.com"
  zone = var.web_zone
  model = "Web"
  size = "S"
  instance = count.index+1
}

resource "adaptive_vip" "hlb1" {
  zone = var.web_zone
  adresses = adaptive_server.web-server1[*].fe_ip
}

resource "adaptive_firewall_rule" "fw_rule1" {
  count=var.server_count
  source_adress = adaptive_server.web-server1[count.index].fe_ip //adaptive_server.web-server1[*].fe_ip
  source_port = 123
  dest_adress = adaptive_server.web-server1[count.index].be_ip //adaptive_server.web-server1[*].fe_ip
  dest_port = "456"
}


provider "adaptive" {
  endpoint = "https://localhost:5001/privatecloud/provider"
}
