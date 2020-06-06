#dotnet publish -c debug --self-contained --runtime linux-arm
dotnet build RpiProbeLogger\\RpiProbeLogger.csproj
if ($?) {
	scp -rp RpiProbeLogger\\bin\Debug\netcoreapp3.1 pi@raspberrypi.local:~/RpiProbeLogger
	#scp -rp RpiProbeLogger\bin\release\netcoreapp3.1\linux-arm\publish\ pi@raspberrypi.local:~/RpiProbeLogger
}