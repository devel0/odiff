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

## usage

this tool is useful to compare two big files with minor changes between and to view them

```sh
devel0@tuf:~$ odiff -s -d /home/devel0/tmp/test1 /home/devel0/tmp/test | less

                                 file1       file2
--------------------------------------------------
max changed chars per line           1           1
size                         120850129   120506488
lines                           343641      343641
line differs                    343641      343641
line differs (%)                 100.0       100.0
char differs                    343641           0
char differs (%)                   0.3         0.0

file1 | file2
,     |      
,     |      
,     |      
,     |      
,     |      
,     |      
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
