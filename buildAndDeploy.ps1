#Requires -Modules Posh-SSH
# Posh-SSH will user password to decrypt key with passphrase
# SSH Session to run bash commands over SSH
$sshSession = Get-SSHSession

# SFTP Session to copy build result
$sftpSession = Get-SFTPSession
if (!$sshSession -Or !$sftpSession)
{
		$credentials = (Get-Credential pi)
}

if(!$sshSession)
{
	$sshSession = New-SSHSession -Computer raspberrypi.local -Credential $credentials -KeyFile '~/.ssh/personal'
}

if(!$sftpSession)
{
	$sftpSession = New-SFTPSession -Computer raspberrypi.local -Credential $credentials -KeyFile '~/.ssh/personal'
}

dotnet build
# Copy build result if no errors
if ($?) {
	Set-SFTPFolder -SFTPSession $sftpSession -RemotePath '/home/pi/RpiProbeLogger' -LocalFolder 'RpiProbeLogger\bin\Debug\netcoreapp3.1' -Overwrite
	Invoke-SSHCommand -Command 'sudo rm /etc/systemd/system/probelogger.service' -SSHSession $sshSession
	Invoke-SSHCommand -Command 'sudo ln -s /home/pi/RpiProbeLogger/probelogger.service /etc/systemd/system/probelogger.service' -SSHSession $sshSession
}