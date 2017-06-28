param(
	[Parameter(Mandatory=$True)]
    [string]$versionNumber,
	[Parameter(Mandatory=$True)]
    [string]$commitId ,
    [string]$releaseNotes = "",
	[Parameter(Mandatory=$True)]
    [string]$artifactfilepath ,
	[Parameter(Mandatory=$True)]
    [string]$gitHubUsername ,
    [Parameter(Mandatory=$True)]
    [string]$gitHubApiKey  ,
	[Parameter(Mandatory=$True)]
    [string]$gitHubRepository ,
	[Boolean]$isDraft = $false,
	[Boolean]$isPreRelease = $false	
)



Write-Host "";
Write-Host "Create new relase on github";
Write-Host "";
Write-Host "   versionNumber : "$versionNumber;
Write-Host "        commitId : "$commitId;
Write-Host "    releaseNotes : "$releaseNotes;
Write-Host "artifactfilepath : "$artifactfilepath;
Write-Host "  gitHubUsername : "$gitHubUsername;
Write-Host "gitHubRepository : "$gitHubRepository;
Write-Host "         isDraft : "$isDraft;
Write-Host "    isPreRelease : "$isDraft;
Write-Host "";


$filename = ([System.IO.Path]::GetFileName(($artifactfilepath)));

$releaseObject = @{
   tag_name = [string]::Format("v{0}", $versionNumber);
   target_commitish = $commitId;
   name = [string]::Format("Version {0}", $versionNumber);
   body = $releaseNotes;
   draft = $isDraft;
   prerelease = $isDraft;
 }


 $releaseParams = @{
   Uri = "https://api.github.com/repos/$gitHubUsername/$gitHubRepository/releases";
   Method = 'POST';
   Headers = @{
     Authorization = 'Basic ' + [Convert]::ToBase64String(
     [Text.Encoding]::ASCII.GetBytes($gitHubApiKey + ":x-oauth-basic"));
   }
   ContentType = 'application/json';
   Body = (ConvertTo-Json $releaseObject -Compress)
 }
Write-Host "releaseParams";
Write-Host (ConvertTo-Json $releaseObject);


$result = Invoke-RestMethod @releaseParams 

$uploadUri = $result | Select-Object -ExpandProperty upload_url
$uploadUri = $uploadUri -replace '\{\?name,label\}', "?name=$filename"

Write-Host "upload file to : "$uploadUri;

$uploadParams = @{
   Uri = $uploadUri;
   Method = 'POST';
   Headers = @{
     Authorization = 'Basic ' + [Convert]::ToBase64String(
     [Text.Encoding]::ASCII.GetBytes($gitHubApiKey + ":x-oauth-basic"));
   }
   ContentType = 'application/zip';
   InFile = $artifactfilepath
 }

 $result = Invoke-RestMethod @uploadParams