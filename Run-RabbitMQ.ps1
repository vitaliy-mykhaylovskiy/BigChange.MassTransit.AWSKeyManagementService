Write-Output "Downloading RabbitMQ 3.7.5"

$tempPath = (Join-Path ([System.IO.Path]::GetTempPath())([System.Guid]::NewGuid()))
New-Item $tempPath -Type Directory

Invoke-WebRequest -Uri "https://github.com/rabbitmq/rabbitmq-server/releases/download/v3.7.5/rabbitmq-server-windows-3.7.5.zip" -OutFile "$tempPath/rabbitmq-server-windows-3.7.5.zip"

Write-Output "Extracting RabbitMQ Archive"

Expand-Archive "$tempPath/rabbitmq-server-windows-3.7.5.zip" -DestinationPath "$tempPath/rabbitmq-server-windows"

Write-Output "Running RabbitMQ"

Start-Process "$tempPath/rabbitmq-server-windows/rabbitmq_server-3.7.5/sbin/rabbitmq-server.bat" -WindowStyle Hidden
