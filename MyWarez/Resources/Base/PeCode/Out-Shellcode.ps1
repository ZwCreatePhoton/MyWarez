Param (
    [Parameter(Position = 0, Mandatory = $True)]
    [String]
    $InputExe,

    [Parameter(Position = 1, Mandatory = $True)]
    [ValidateScript({ Test-Path $_ })]
    [String]
    $ProjectDir,

    [Parameter(Position = 2, Mandatory = $True)]
    [String]
    $OutputFile
)

$GetPEHeader = Join-Path $ProjectDir Get-PEHeader.ps1

. $GetPEHeader

$PE = Get-PEHeader $InputExe -GetSectionData
$TextSection = $PE.SectionHeaders | Where-Object { $_.Name -eq '.text' }

[IO.File]::WriteAllBytes($OutputFile, $TextSection.RawData)
