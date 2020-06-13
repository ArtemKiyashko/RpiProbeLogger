#Requires -Modules Posh-SSH

param([switch]$enableService=$false, [switch]$runService=$false, $c='Debug')

$rsaKeyFile = '~/.ssh/personal'
# Posh-SSH will use password to decrypt key with passphrase
# SSH Session to run bash commands over SSH
$sshSession = Get-SSHSession

# SFTP Session to copy build result
$sftpSession = Get-SFTPSession
if (!$sshSession -Or !$sshSession.Connected -Or !$sftpSession -Or !$sftpSession.Connected)
{
		$credentials = (Get-Credential pi)
}

if(!$sshSession)
{
	$sshSession = New-SSHSession -Computer raspberrypi.local -Credential $credentials -KeyFile $rsaKeyFile
}

if(!$sftpSession)
{
	$sftpSession = New-SFTPSession -Computer raspberrypi.local -Credential $credentials -KeyFile $rsaKeyFile
}

dotnet publish RpiProbeLogger\RpiProbeLogger.csproj --self-contained -r linux-arm -f netcoreapp3.1 -c $c
# Copy build result if no errors
if ($?) {
	Set-SFTPFolder -SFTPSession $sftpSession -RemotePath '/home/pi/RpiProbeLogger' -LocalFolder "RpiProbeLogger\bin\$($c)\netcoreapp3.1\linux-arm\publish" -Overwrite
	Invoke-SSHCommand -Command 'chmod +x /home/pi/RpiProbeLogger/RpiProbeLogger' -SSHSession $sshSession
	if ($enableService)
	{
		Invoke-SSHCommand -Command 'sudo systemctl enable /home/pi/RpiProbeLogger/probelogger.service' -SSHSession $sshSession
	}
	else {
		Invoke-SSHCommand -Command 'sudo systemctl disable /home/pi/RpiProbeLogger/probelogger.service' -SSHSession $sshSession
	}
	if ($enableService -And $runService)
	{
		Invoke-SSHCommand -Command 'sudo systemctl start probelogger.service' -SSHSession $sshSession
	}
	else {
		Invoke-SSHCommand -Command 'sudo systemctl stop probelogger.service' -SSHSession $sshSession
	}
}