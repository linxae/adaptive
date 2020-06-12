Get-ChildItem .\terraform.tfstate | Remove-Item -Force -ErrorAction SilentlyContinue
Get-ChildItem .\..\..\tests\.output\*.json | Remove-Item -Force -ErrorAction SilentlyContinue

