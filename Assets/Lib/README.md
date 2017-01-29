Lib
===

Jammer.dll
------------

Jammer base classes

### Jammer.Logger

Granular Unity debug log wrapper with source (./src) but compiled to DLL to
avoid wrapper functions junking up your stack traces.  Conditionally compiled
to allow complete trace logging removal for production builds.  Rebuild via
`thor compile:base` See [./src](../../src) for source code.
