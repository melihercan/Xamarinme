Normally this kind of apps will include the 'WebHostPatch' NuGet that overrides the Microsoft.Extensions.Primitives
assembly.
If project is included instaed of NuGet (debugging for example), iOS build complains that it cannot find v2.2.0 of
primitives. Therefore use debugging only on Android with this format.