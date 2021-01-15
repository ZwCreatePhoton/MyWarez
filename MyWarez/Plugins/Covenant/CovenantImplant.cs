using Covenant.Core;
using Covenant.Models;
using Covenant.Models.Grunts;
using Covenant.Models.Launchers;
using Covenant.Models.Listeners;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis; // Required package reference ;OutputKind
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace MyWarez.Plugins.Covenant
{
    public sealed class CovenantImplant
    {
        public CovenantImplant(Grunt grunt, Profile profile = null, Launcher launcher = null)
        {
            Grunt = grunt;
            Profile = profile;
            Launcher = launcher;
        }

        public CovenantImplant(string taskName, string taskParameters, string outfile = "", Common.DotNetVersion dotnetVersion = Common.DotNetVersion.Net40, Platform platform = Platform.AnyCpu)
        {
            Listener listener = new global::Covenant.Models.Listeners.HttpListener();
            string profileFilePath = Common.CovenantProfileDirectory + "DefaultHttpProfile.yaml";
            HttpProfile profile = HttpProfile.Create(profileFilePath);

            ImplantTemplate template = Service.GetImplantTemplateByName("GruntTaskNone").Result;

            GruntTask task = Service.GetGruntTaskByName(taskName).Result;
            task.Platform = platform;

            Launcher launcher = new Launcher();

            Grunt grunt = new Grunt
            {
                ListenerId = listener.Id,
                Listener = listener,
                ImplantTemplateId = template.Id,
                ImplantTemplate = template,
                TaskId = task.Id,
                Task = task,
                TaskParameterString = taskParameters,
                DotNetVersion = dotnetVersion,
                Platform = platform,
                OutputFile = outfile,

                //defaults
                SMBPipeName = launcher.SMBPipeName,
                ValidateCert = launcher.ValidateCert,
                UseCertPinning = launcher.UseCertPinning,
                Delay = launcher.Delay,
                JitterPercent = launcher.JitterPercent,
                ConnectAttempts = launcher.ConnectAttempts,
                KillDate = launcher.KillDate,
            };

            Grunt = grunt;
            Profile = profile;
        }

        public Grunt Grunt { get; }
        public Profile Profile { get; }
        public Launcher Launcher { get; private set; }

        private static void SetupCovenantServiceContext()
        {
            string CovenantBindUrl = "0.0.0.0";
            IPAddress address = null;
            try
            {
                address = IPAddress.Parse(CovenantBindUrl);
            }
            catch (FormatException)
            {
                address = Dns.GetHostAddresses(CovenantBindUrl).FirstOrDefault();
            }
            IPEndPoint CovenantEndpoint = new IPEndPoint(address, Common.CovenantHTTPSPort);
            string CovenantUri = CovenantBindUrl == "0.0.0.0" ? "https://127.0.0.1:" + Common.CovenantHTTPSPort : "https://" + CovenantEndpoint;
            var host = global::Covenant.Program.BuildHost(CovenantEndpoint, CovenantUri);
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CovenantContext>();
            var service = services.GetRequiredService<ICovenantService>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var listenerTokenSources = services.GetRequiredService<ConcurrentDictionary<int, CancellationTokenSource>>();
            context.Database.EnsureCreated();
            DbInitializer.Initialize(service, context, roleManager, listenerTokenSources).Wait();
            _context = context;
            _service = (CovenantService)service;
        }


        private static CovenantService _service = null;
        public static CovenantService Service
        {
            get
            {
                if (_service == null)
                {
                    SetupCovenantServiceContext();
                    return _service;
                }
                else
                {
                    return _service;
                }
            }
        }

        private static CovenantContext _context = null;
        public static CovenantContext Context
        {
            get
            {
                if (_context == null)
                {
                    SetupCovenantServiceContext();
                    return _context;
                }
                else
                {
                    return _context;
                }
            }
        }

        public void SetLauncher(Launcher launcher)
        {
            var grunt = Grunt;
            var listener = grunt.Listener;
            var template = grunt.ImplantTemplate;
            var task = grunt.Task;

            launcher.ListenerId = listener.Id;
            launcher.ImplantTemplateId = template.Id;
            launcher.TaskId = task.Id;
            launcher.DotNetVersion = grunt.DotNetVersion;

            Launcher = launcher;
        }

        public void Compile()
        {
            var grunt = Grunt;
            var listener = grunt.Listener;
            var template = grunt.ImplantTemplate;
            var task = grunt.Task;
            var profile = Profile;
            var launcher = Launcher;

            launcher.GetLauncher(
                Service.GruntTemplateReplace(template.StagerCode, template, grunt, listener, profile),
                Service.CompileGruntCode(template.StagerCode, template, grunt, listener, profile, launcher.OutputKind, launcher.CompressStager),
                grunt,
                template
            );
        }
        private static int optionIdCount = 20000;
        // code should contain 
        public static void AddTask(string taskName, string code, int optionCount = 0, string[] referenceDlls = null)
        {
            bool throwExcep = false;
            try
            {
                GruntTask t = Service.GetGruntTaskByName(taskName).Result;
                if (t != null)
                    throwExcep = true;
            }
            catch { }

            if (throwExcep)
                throw new ArgumentException("Task already exists!");

            var options = new List<GruntTaskOption>();
            for (int i = 0; i < optionCount; i++)
            {
                var option = new GruntTaskOption
                {
                    Id = optionIdCount,
                    //Name = MyWarez.Utils.RandomString(5),
                    Optional = false,
                };
                options.Add(option);
                optionIdCount++;
            }


            var task = new GruntTask
            {
                Name = taskName,
                Code = code,
                Options = options
            };

            var references = new List<GruntTaskReferenceAssembly>()
            {
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("mscorlib.dll", Common.DotNetVersion.Net35).Result },
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("System.dll", Common.DotNetVersion.Net35).Result },
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("System.Core.dll", Common.DotNetVersion.Net35).Result },
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("mscorlib.dll", Common.DotNetVersion.Net40).Result },
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("System.dll", Common.DotNetVersion.Net40).Result },
                new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName("System.Core.dll", Common.DotNetVersion.Net40).Result },
            };
            foreach (var referenceDll in referenceDlls)
            {
                if ((new string[] { "mscorlib.dll", "System.dll", "System.Core.dll" }).Contains(referenceDll))
                    continue;
                bool yes35 = true;
                bool yes40 = true;

                if (referenceDll == "System.Runtime.InteropServices.dll")
                    yes35 = false;

                if (yes35)
                {
                    var reference35 = new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName(referenceDll, Common.DotNetVersion.Net35).Result };
                    references.Add(reference35);
                }
                if (yes40)
                {
                    var reference40 = new GruntTaskReferenceAssembly { GruntTask = task, ReferenceAssembly = Service.GetReferenceAssemblyByName(referenceDll, Common.DotNetVersion.Net40).Result };
                    references.Add(reference40);
                }
            }

            Context.GruntTasks.AddRange(task);
            foreach (var reference in references)
                Context.AddRange(reference);
            Context.SaveChanges();
        }
    }
}
