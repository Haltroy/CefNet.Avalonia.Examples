# CefNet.Avalonia.Examples

This repository contains 2 examples.

 - CefNet.Avalonia.Example: The one used in the CefNet repository, but project references are replaced with package references.
 - CefNet.AvaloniaMVVM.Example: Super-simple example for testing with Model-View-View-Model projects. You can use this one to build your app on.

Current CEF Version: `105.3.18+gbfea4e1+chromium-105.0.5195.52`

## BUILD

1. Download CEF binaries:
     - [Windows 32-bit](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_windows32_beta_client.tar.bz2)
     - [Windows 64-bit](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_windows64_beta_client.tar.bz2)
     - [Windows ARM 64](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_windows3arm64_beta_client.tar.bz2)
     - [Linux 64-bit](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_linux64_beta_client.tar.bz2)
     - [Linux ARM](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_linuxarm_beta_client.tar.bz2)
     - [Linux ARM 64](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_linuxarm64_beta_client.tar.bz2)
     - [MacOS 64-bit Intel/AMD](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_macosx64_beta_client.tar.bz2)
     - [MacOS ARM](https://cef-builds.spotifycdn.com/cef_binary_105.3.18%2Bgbfea4e1%2Bchromium-105.0.5195.52_macosarm64_beta_client.tar.bz2)
     - Fun fact: All these should have their own `cefclient` application. You can test if CEF is working by running the application. If it crashes than re-download it.
2. Extract them to `[your home folder]/.cefnet`. The `.cefnet` folder should contain a `Release` folder.
     - These files are all TAR files, use 7-Zip or equivalent program (for Windows) or `tar xf [downloaded file] -C ~/.cefnet` command (for the rest) to extract them.
3. Run any one of the examples by running `dotnet run` command.
     ```bash
        git clone https://github.com/haltroy/CefNet.Avalonia.Examples.git
        cd CefNet.Avalonia.Examples
       
       # Run an example
        cd [any example you prefer]
        dotnet run
        ```

## Known Issues

1. Issue: `System.DllNotFoundException: Can't load [some path]/libcef.so`
    - Solution: `export LD_PRELOAD=[that path]/libcef.so:$LD_PRELOAD`
2. Issue: `System.DllNotFoundException: Unable to load shared library 'libdl'`
    - Solution 1: `sudo ln -sf /usr/lib64/$(ls /usr/lib64/ | grep libdl.so) /usr/lib64/libdl.so`
    - Solution 2: `whereis libdl.so.2` -> `sudo ln -sf <your_libdl.so.2_path> /usr/lib/libdl.so`
    - Solution 3: Install GNU C libraries for your Linux distro.
    - Note: All solutions should work.
