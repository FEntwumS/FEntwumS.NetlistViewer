# Purpose

This document describes the complete process for releasing a new version of the FEntwumS Netlist Viewer Extension for
OneWare Studio.

# Creating a new release

1. Create a standalone commit modifying the `<Version>` entry in
`src/FEntwumS.NetlistViewer/FEntwumS.NetlistViewer.csproj` to reflect the intended version to be 
released
   1. Version numbers need to be in the format `major.minor.patch`
   2. Version numbers may only contain numeric symbols as well as `.` as separator
2. Tag the commit using the new version number
3. Push the commit to the master branch
4. Create a new release on https://github.com/FEntwumS/FEntwumS.NetlistViewer/releases
   1. Use the just created tag as base for the release
   2. Use the tag of the previous release as comparison
   3. Release notes may be auto generated or manually written
   4. Make sure the release is tagged as the latest release
5. Wait until the release pipeline has finished building and publishing the release artifact (this should take about a
minute)
6. If the release pipeline successfully finishes, add a new entry to `oneware-extension.json` to make the new version
discoverable for OneWare Studio users

# Updating the dependency license information overrides

The release pipeline fails if the `nuget-license` tool can't find licensing information for any dependency.
Such an issue can be resolved by manually adding the necessary information.

After installing the `nuget-license` tool (`dotnet tool install --global nuget-license --version 3.0.14`), run the
following command to generate the license information output:

```
nuget-license -i ./src/FEntwumS.NetlistViewer/FEntwumS.NetlistViewer.csproj -override ./src/FEntwumS.NetlistViewer/override-third-party.json -t -o Table -d ./publish/THIRD-PARTY-LICENSES -fo ./publish/THIRD-PARTY-NOTICE.txt
```

The error column in `publish/THIRD-PARTY-NOTICE.txt` contains the errors for each erroring license.
Resolve these errors by adding a new or modifying an existing override in
`src/FEntwumS.NetlistViewer/override-third-party.json`.

After verifying that the `nuget-license` tool finishes successfully, remove the broken release from GitHub and delete
the corresponding git tag.
Then create a new commit with the modified license overrides and tag it using the version intended for the release.
From here, the normal release procedure can be followed.