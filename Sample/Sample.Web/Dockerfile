#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

#复制当前目录所有 到文件夹YouApp
COPY . /KnownSample

#工作目录
WORKDIR /KnownSample

#暴露端口
EXPOSE 8080
EXPOSE 8081

#入口点
ENTRYPOINT ["dotnet", "Sample.Web.dll"]