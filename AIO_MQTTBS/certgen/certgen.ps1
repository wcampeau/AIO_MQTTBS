#Requires -RunAsAdministrator

param (
    [Parameter(Mandatory=$true)][string]$HostName
#    [string]$password = $( Read-Host "Input PFX password, please" )
 )

# REVIEW: better password? Provide a way for the customer to specify the password on the PFX file?
 $password = "password"
 
 New-Item $HostName -type directory
 Push-Location $HostName
 
# make certificate authority
# REMARK: this will display a dialog asking for an administrator password
makecert -n "CN=MyCARoot" -r -a sha512 -len 4096 -cy authority -sv MyCARoot.pvk MyCARoot.cer

# make server certificate
# REMARK: use the password used in the previous step
makecert -n "CN=$HostName" -iv MyCARoot.pvk -ic MyCARoot.cer -pe -a sha512 -len 4096 -sky exchange -sv GnatMQ.pvk GnatMQ.cer

# export certificate as PFX file
# REMARK: use the password used in the previous step
pvk2pfx -pvk GnatMQ.pvk -spc GnatMQ.cer -pfx GnatMQ.pfx -po $password

Pop-Location