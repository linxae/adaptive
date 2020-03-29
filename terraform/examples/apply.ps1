cd examples
Set-Item env:\TF_LOG -Value TRACE
Set-Item env:\TF_VAR_adaptive_config_endpoint "https://localhost:5001/privatecloud"
terraform init
terraform plan -refresh
terraform destroy -auto-approve
terraform apply -auto-approve
