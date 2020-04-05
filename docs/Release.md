# Release

This document describes how to create a new release for KeeTrayTotp.

1. Bump the `AssemblyVersion` and `AssemblyFileVersion` in `KeeTrayTOTP\Properties\AssemblyInfo.cs`
2. Bump the version in `version_manifest.txt`.
3. Commit these two files, and tag the commit as `vMajor.Minor.Build` (e.g. v1.0.0), and push the commit with tags.
4. A github action "KeeTrayTOTP Tagged Release" will trigger based on the version tag. 
   * [ ] Check that the action succeeds.
   * [ ] Check that a release with the new version is created
5. Create a PR and merge it to master, so the version manifest becomes visible to the Keepass Update Checker.
