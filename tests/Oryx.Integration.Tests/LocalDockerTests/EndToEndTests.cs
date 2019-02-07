﻿// --------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// --------------------------------------------------------------------------------------------

using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Oryx.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Oryx.Integration.Tests.LocalDockerTests
{
    public class EndToEndTests
    {
        private const int HostPort = 8000;
        private const string startupFilePath = "/tmp/startup.sh";

        private readonly ITestOutputHelper _output;
        private readonly string _hostSamplesDir;
        private readonly HttpClient _httpClient;

        public EndToEndTests(ITestOutputHelper output)
        {
            _output = output;
            _hostSamplesDir = Path.Combine(Directory.GetCurrentDirectory(), "SampleApps");
            _httpClient = new HttpClient();
        }

        [Theory]
        [InlineData("10.10")]
        [InlineData("10.14")]
        public async Task NodeApp(string nodeVersion)
        {
            // Arrange
            var appName = "webfrontend";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:80";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath}")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Say It Again", data);
                });
        }

        [Fact]
        public async Task NodeApp_WithYarnLock()
        {
            // Arrange
            var appName = "webfrontend-yarnlock";
            var nodeVersion = "10.14";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:80";
            var startupFile = "/tmp/startup.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Say It Again", data);
                });
        }

        [Fact]
        public async Task NodeApp_WithYarnLock_AndNpmBuild()
        {
            // Arrange
            var appName = "TailwindTraders-Opener";
            var nodeVersion = "10.14";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:3000";
            var startupFile = "/tmp/startup.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/browse");
                    Assert.Contains("<!DOCTYPE html><html lang=\"en\">", data);
                });
        }

        // Run on Linux only as TypeScript seems to create symlinks and this does not work on Windows machines.
        [EnableOnPlatform("LINUX")]
        public async Task NodeApp_BuildNodeUnderScripts()
        {
            // Arrange
            var appName = "NodeAndTypeScriptHelloWorld";
            var nodeVersion = "10.14";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:3000";
            var startupFile = "/tmp/startup.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("{\"message\":\"Hello World!\"}", data);
                });
        }

        [Fact]
        public async Task Node_Lab2AppServiceApp()
        {
            // Arrange
            var appName = "lab2-appservice";
            var nodeVersion = "10.14";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:3000";
            var startupFile = "/tmp/startup.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Welcome to Express", data);
                });
        }

        [Fact]
        public async Task Node_SoundCloudNgrxApp()
        {
            // Arrange
            var appName = "soundcloud-ngrx";
            var nodeVersion = "8.11";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:3000";
            var startupFile = "./run.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir}")
                .AddCommand($"npm rebuild node-sass") //remove this once workitem 762584 is done
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var response = await _httpClient.GetAsync($"http://localhost:{HostPort}/");
                    Assert.True(response.IsSuccessStatusCode);
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("<title>SoundCloud • Angular2 NgRx</title>", data);
                });
        }

        [Fact]
        public async Task Node_CreateReactAppSample()
        {
            // Arrange
            var appName = "create-react-app-sample";
            var nodeVersion = "10.14";
            var hostDir = Path.Combine(_hostSamplesDir, "nodejs", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:3000";
            var startupFile = "/tmp/startup.sh";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "nodejs", "--language-version", nodeVersion },
                $"oryxdevms/node-{nodeVersion}",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("<title>React App</title>", data);
                });
        }

        [Fact]
        public async Task Python27App()
        {
            // Arrange
            var appName = "python2-flask-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var startupFile = "/tmp/startup.sh";
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile} -hostBind=\":5000\"")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "python", "--language-version", "2.7" },
                "oryxdevms/python-2.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Hello World!", data);
                });
        }

        [Fact]
        public async Task Python27App_virtualEnv()
        {
            // Arrange
            var appName = "python2-flask-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            const string virtualEnvName = "antenv2.7";
            var startupFile = "/tmp/startup.sh";
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                // Mimic the commands ran by app service in their derived image.
                .AddCommand("pip install gunicorn")
                .AddCommand("pip install flask")
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFile} -hostBind=\":5000\" -virtualEnvName={virtualEnvName}")
                .AddCommand(startupFile)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "python", "--language-version", "2.7", "-p", $"virtualenv_name={virtualEnvName}" },
                "oryxdevms/python-2.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Hello World!", data);
                });
        }

        [Fact]
        public async Task FlaskApp_Python37()
        {
            // Arrange
            var appName = "flask-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=\":5000\"")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir },
                "oryxdevms/python-3.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Hello World!", data);
                });
        }

        [Fact]
        public async Task FlaskApp_Python36()
        {
            // Arrange
            var appName = "flask-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=:5000")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "python", "--language-version", "3.6" },
                "oryxdevms/python-3.6",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("Hello World!", data);
                });
        }

        [Fact]
        public async Task DjangoApp_Python37()
        {
            // Arrange
            var appName = "django-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=:5000")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir },
                "oryxdevms/python-3.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/boards.css");
                    Assert.Contains("CSS file from Boards app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/uservoice.css");
                    Assert.Contains("CSS file from UserVoice app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/boards/");
                    Assert.Contains("Hello, World! from Boards app", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/uservoice/");
                    Assert.Contains("Hello, World! from Uservoice app", data);
                });
        }

        [Fact]
        public async Task DjangoApp_Python37_virtualenv()
        {
            // Arrange
            var appName = "django-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            const string virtualEnvName = "antenv";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=:5000 -virtualEnvName={virtualEnvName}")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-p", $"virtualenv_name={virtualEnvName}", "-l", "python", "--language-version", "3.7" },
                "oryxdevms/python-3.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/boards.css");
                    Assert.Contains("CSS file from Boards app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/uservoice.css");
                    Assert.Contains("CSS file from UserVoice app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/boards/");
                    Assert.Contains("Hello, World! from Boards app", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/uservoice/");
                    Assert.Contains("Hello, World! from Uservoice app", data);
                });
        }

        [Fact]
        public async Task DjangoApp_Python36()
        {
            // Arrange
            var appName = "django-app";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=:5000")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "python", "--language-version", "3.6" },
                "oryxdevms/python-3.6",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/boards.css");
                    Assert.Contains("CSS file from Boards app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/staticfiles/css/uservoice.css");
                    Assert.Contains("CSS file from UserVoice app module", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/boards/");
                    Assert.Contains("Hello, World! from Boards app", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/uservoice/");
                    Assert.Contains("Hello, World! from Uservoice app", data);
                });
        }

        [Fact]
        public async Task Tweeter3_Python37()
        {
            // Arrange
            var appName = "tweeter3";
            var hostDir = Path.Combine(_hostSamplesDir, "python", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var containerPort = 8000;
            var portMapping = $"{HostPort}:{containerPort}";
            var script = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath} -hostBind=\":{containerPort}\"")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir },
                "oryxdevms/python-3.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    script
                },
                async () =>
                {
                    var data = await _httpClient.GetStringAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("logged in as: bob", data);
                });
        }

        [Fact]
        public async Task ReactAndDotNet()
        {
            // Arrange
            var appName = "dotnetreact";
            var hostDir = Path.Combine(_hostSamplesDir, "multilanguage", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:5000";
            var runAppScript = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddCommand($"oryx -sourcePath {appDir} -output {startupFilePath}")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "oryx",
                new[] { "build", appDir, "-l", "dotnet", "--language-version", "2.2" },
                "oryxdevms/dotnetcore-2.2",
                portMapping,
                "/bin/sh",
                new[]
                {
                    "-c",
                    runAppScript
                },
                async () =>
                {
                    var data = await GetResponseDataAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("src=\"/static/js/main", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/static/js/main.df777a6e.js");
                    Assert.Contains("!function(e){function t(o){if(n[o])return", data);
                });
        }

        [Fact]
        public async Task ReactAndDjango()
        {
            // Arrange
            var appName = "reactdjango";
            var hostDir = Path.Combine(_hostSamplesDir, "multilanguage", appName);
            var volume = DockerVolume.Create(hostDir);
            var appDir = volume.ContainerDir;
            var portMapping = $"{HostPort}:8000";

            var buildScript = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                .AddBuildCommand($"{appDir} -l python --language-version 3.7")
                .ToString();

            var runAppScript = new ShellScriptBuilder()
                .AddCommand($"cd {appDir}")
                // User would do this through app settings
                .AddCommand("export DJANGO_SETTINGS_MODULE=\"reactdjango.settings.local\"")
                .AddCommand($"oryx -appPath {appDir} -output {startupFilePath}")
                .AddCommand(startupFilePath)
                .ToString();

            await EndToEndTestHelper.BuildRunAndAssertAppAsync(
                appName,
                _output,
                volume,
                "/bin/bash",
                new[]
                {
                    "-c",
                    buildScript
                },
                "oryxdevms/python-3.7",
                portMapping,
                "/bin/bash",
                new[]
                {
                    "-c",
                    runAppScript
                },
                async () =>
                {
                    var data = await GetResponseDataAsync($"http://localhost:{HostPort}/");
                    Assert.Contains("<h1>it works! (Django Template)</h1>", data);

                    data = await GetResponseDataAsync($"http://localhost:{HostPort}/static/webpack_bundles/main-137f65fb0e0a8e523666.js");
                    Assert.Contains("!function(e){var t={};function n(r){if(t[r])return t[r].exports", data);
                });
        }

        // The following method is used to avoid following exception from HttpClient when trying to read a response:
        // '"utf-8"' is not a supported encoding name. For information on defining a custom encoding,
        // see the documentation for the Encoding.RegisterProvider method.
        private async Task<string> GetResponseDataAsync(string url)
        {
            var bytes = await _httpClient.GetByteArrayAsync(url);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}