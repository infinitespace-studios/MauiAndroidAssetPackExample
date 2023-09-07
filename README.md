# MauiAndroidAssetsExample
An Example of using Dynamic Asset Delivery with Xamarin Android.

## Project Layout

The solution is made up of 2 projects.

1. InstallTimeExample. This is the main Maui application for the
   InstallTime Example.
2. AssetsFeature. This is the project for the Asset only dynamic feature.

There is a `global.json` file which pulls in the `Microsoft.Build.NoTargets` SDK
which we use for the "Feature" projects. This is so they do not produce an assembly. 

## How it works.

The way the build works is we have a custom set of targets in the `Targets\DynamicFeature.targets`
file. The two targets are `BuildAssetFeature` and `PackageAssets`. The first target
is build as part of the main Xamarin.Android application. It is responsible for 
finding "Feature" projects and then calling the `PackageAssets` target on each of 
them. 

It does this by looking for `ProjectReferences` which have the `DynamicFeature` metadata
set to `true`.

```
    <ProjectReference Include="..\AssetsFeature\AssetsFeature.csproj">
      <Project>{EABACE4D-E999-48FA-B417-ECD29C8AB6E5}</Project>
      <Name>AssetsFeature</Name>
      <!-- These next two items are REALLY IMPORTANT!!!! -->
      <DynamicFeature>true</DynamicFeature>
      <ReferenceOutputAssembly>false</ReferenceOutputAssembly>
    </ProjectReference>
```

NOTE: Feature references should ALSO have the `ReferenceOutputAssembly` set to false. This 
stops the Xamarin.Android packaging system from including it in the base package.

The `PackageAssets` target will run for each "feature" project. It is responsible for 
using `aapt2` to package up the files in the `Assets` folder (including subdirectories)
and generating a "Feature" package/zip file. 

The outputs of `PackageAssets` are then passed back to `BuildAssetFeature` which then includes
those zip files in the `@(AndroidAppBundleModules)` ItemGroup. These will then be included
in the final `aab` file as dynamic features. 

## Defining an Asset Pack

To create a feature you need a few things. The first is a `Microsoft.Build.NoTargets` project
which imports the `Targets\DynamicFeatures.targets` file. See `OnDemand\AssetFeature\AssetFeature.csproj` or `InstallTime\AssetFeature\AssetFeature.csproj`  for an example.

Next you need an `AndroidManifest.xml` file. This is where you define how they "Feature" will be
installed via the `dist:module` and `dist:delivery` elements. 

IMPORTANT: The `dist:type` MUST be set to `asset-pack`!

The `package` attribute on the `manifest` element MUST match the value of the main application.
And finally the `split` value is the name which the "Feature" will be called in the final package
and when you install via the `AssetPackManager` API. This is not user facing. 

## Installing and Install Time Asset Pack

Install time asset packs as installed along side your app during the installation 
process. There is no additional work needed to download them.

Accessing these assets can be done via the normal `OpenAppPackageFileAsync` method.

```
using Stream fileStream = await FileSystem.Current.OpenAppPackageFileAsync("Foo.txt");
```