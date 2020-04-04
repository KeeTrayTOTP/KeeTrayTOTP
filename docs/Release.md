# Release

This document describes how to create a new release for KeeTrayTotp.
For now this process is done manually.

1. Bump the `AssemblyVersion` and `AssemblyFileVersion` in `KeeTrayTOTP\Properties\AssemblyInfo.cs`
2. Bump the version in `version_manifest.txt`. 
3. Create a PR containing the `AssemblyInfo.cs` and `version_manifest.txt` and merge it to master.
4. A [github action](https://github.com/victor-rds/KeeTrayTOTP/actions?query=branch%3Amaster+workflow%3A%22KeeTrayTOTP+CI+Build%22) will fire on the master branch, producing `KeeTrayTOTP.plgx` under artifacts after a successful build. 
4. Go to [Releases](https://github.com/victor-rds/KeeTrayTOTP/releases) and draft a new release.
   * Tag version:	Major.Minor-Beta (e.g. 0.101-Beta)
   * Release title: Version Major.Minor-Beta (e.g. Version 0.101-Beta)
   * Gather the changes done since the last release, referencing issues / PR's and mentioning contributions.
   * Attach the `KeeTrayTOTP.plgx`
   * Check the "This is a pre-release" checkbox.
