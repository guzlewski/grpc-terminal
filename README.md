# gRPC-Terminal
gRPC-Terminal is client and server for starting process on server-side and sending back exit code, process output and process errors. It also supports multiline input to stdin (flag --i). Written in C# .NET CORE 3.1, works on Linux and Windows. Server sends back result after process exit.

## Compilation
```
git clone https://github.com/guzlewski/grpc-terminal
cd grpc-terminal
./build.sh or open in Visual Studio
```

## Usage

```
./Server HOST PORT
./Client HOST PORT PROGRAM_NAME [--i] [ARG1] ...
```

## Examples
```
./Server 127.0.0.1 13001
./Client 127.0.0.1 13001 ls -lh -a
```
Send command to server to run ls with flags -l -h -a and send back exit code, stdout and stderr.

 ```
./Server 0.0.0.0 20000
./Client PUBLIC_SERVER_IP 20000 gcc -Wall program.c
```
Send command to server to run gcc and compile file program.c.

 ```
./Server 0.0.0.0 20000
./Client PUBLIC_SERVER_IP 20000 apt-get update
```
Run 'apt-get' program with argument 'update' on server. 

 ```
./Server 0.0.0.0 13005
./Client PUBLIC_SERVER_IP 13005 python --i
help()
EOF
```
Send command to server to run python, write 'help()' to stdin of python process and close pipe.
Waits till process exit and server sends back response.  
EOF is Crtl+D on Linux, Ctrl+Z on Windows.

## License
Copyright (c) guzlewski. All rights reserved.
