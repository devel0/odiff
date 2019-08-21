# odiff

[![NuGet Badge](https://buildstats.info/nuget/odiff)](https://www.nuget.org/packages/odiff/)

OnlyDiff

- [Quickstart](#quickstart)
- [command line](#command-line)
- [test](#test)
- [How this project was built](#how-this-project-was-built)

<hr/>

## Quickstart

- Requirements: [Download NET Core SDK](https://dotnet.microsoft.com/download)
- Install the tool:

```sh
dotnet tool install -g odiff
```

- To update if already installed:

```sh
dotnet tool update -g odiff
```

- if `~/.dotnet/tools` dotnet global tool isn't in path it can be added to your `~/.bashrc`

```sh
echo 'export PATH=$PATH:~/.dotnet/tools' >> ~/.bashrc
```

## command line

```sh

Usage: odiff FLAGS file1 file2

Shows only difference between files

Optional flags:

  -s,--all-stats          shows all stats informations
  -d,--show-differences   shows characters difference

Parameters:

  file1                   first file
  file2                   second file

```

## output example

```
                              file1    file2
--------------------------------------------
max changed chars per line        2        2
size                         227939   231139
lines                          1601     1601
line differs                   1600     1600
line differs (%)               99.9     99.9
char differs                   3200     3200
char differs (%)                1.4      1.4
```

## How this project was built

```sh
mkdir odiff
cd odiff

dotnet new sln
dotnet new console -n odiff

cd odiff
dotnet add package netcore-util --version 1.0.21
cd ..

dotnet sln add odiff
dotnet build
dotnet run -p odiff
```
