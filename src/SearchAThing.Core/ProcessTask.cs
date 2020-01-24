#region SearchAThing.Core, Copyright(C) 2015-2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace SearchAThing
{

    namespace Core
    {

        public enum ProcessTaskPathMode
        {
            /// <summary>
            /// exact full pathname of the executable will be given
            /// </summary>
            ExecutableFullPathname,

            /// <summary>
            /// search in envPath if given, the in PATH
            /// </summary>
            SeachInEnvironment
        };

        /// <summary>
        /// Help to manage pipeline ( stdin, stdout, stderr ) redirection through use of Task.
        /// After object created, set redirections with properties then start it.
        /// Remember to set RedirectStandard{Input,Output,Error} as needed.
        /// </summary>
        public class ProcessTask
        {

            public string Pathfilename { get; private set; }

            public bool RedirectStandardInput { get; set; }
            public bool RedirectStandardOutput { get; set; }
            public bool RedirectStandardError { get; set; }

            public string Arguments { get; set; }

            Process process = null;

            public bool IsActive { get { return process != null; } }

            public ProcessTask(string _pathfilename,
                ProcessTaskPathMode mode = ProcessTaskPathMode.SeachInEnvironment,
                string envPath = null)
            {
                #region ensure Pathfilename exists
                switch (mode)
                {
                    case ProcessTaskPathMode.ExecutableFullPathname:
                        {
                            Pathfilename = _pathfilename;

                            if (!File.Exists(Pathfilename)) throw new Exception($"not found [{Pathfilename}]");
                        }
                        break;

                    case ProcessTaskPathMode.SeachInEnvironment:
                        {
                            _pathfilename = System.IO.Path.GetFileName(_pathfilename);

                            // check in env var if given
                            if (envPath != null)
                            {
                                var path = Environment.GetEnvironmentVariable(envPath);
                                if (path != null) Pathfilename = System.IO.Path.Combine(path, _pathfilename);
                            }

                            if (!File.Exists(Pathfilename))
                            {
                                Pathfilename = _pathfilename.SearchInPath();

                                if (Pathfilename == null)
                                {
                                    if (envPath != null)
                                        throw new Exception($"not found [{_pathfilename}] in given env [{envPath}] neither in PATH");
                                    else
                                        throw new Exception($"not found [{_pathfilename}] in PATH");
                                }
                            }

                        }
                        break;
                }
                #endregion
            }

            public bool Start()
            {
                process = new Process();
                process.StartInfo.FileName = Pathfilename;                
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = RedirectStandardInput;
                process.StartInfo.RedirectStandardOutput = RedirectStandardOutput;
                process.StartInfo.RedirectStandardError = RedirectStandardError;

                return process.Start();
            }

            public void Write(string str, bool flush = true)
            {
                if (process == null) throw new Exception($"process not started");
                if (!process.StartInfo.RedirectStandardInput) throw new Exception($"need to redirect standard input to write to the process");

                process.StandardInput.Write(str);
                if (flush) process.StandardInput.Flush();
            }
            
            bool startedOutputSampling = false;
            public async Task<string> ReadOutput(TimeSpan? timeout = null)
            {
                CancellationToken cancellationToken = CancellationToken.None;
                if (timeout.HasValue)
                    cancellationToken = new CancellationTokenSource(timeout.Value).Token;

                var q = await TaskExt.FromEvent<DataReceivedEventArgs>(
                    handler => process.OutputDataReceived += new DataReceivedEventHandler(handler),
                    () =>
                    {
                        if (!startedOutputSampling)
                        {
                            startedOutputSampling = true;
                            process.BeginOutputReadLine();
                        }
                    },
                    handler => process.OutputDataReceived -= new DataReceivedEventHandler(handler),                    
                    cancellationToken);

                return q.Data;
            }

            bool startedErrorSampling = false;               
            public async Task<string> ReadError()
            {
                var q = await TaskExt.FromEvent<DataReceivedEventArgs>(
                    handler => process.ErrorDataReceived += new DataReceivedEventHandler(handler),
                    () =>
                    {
                        if (!startedErrorSampling)
                        {
                            startedErrorSampling = true;
                            process.BeginErrorReadLine();
                        }
                    },
                    handler => process.ErrorDataReceived -= new DataReceivedEventHandler(handler),
                    CancellationToken.None);

                return q.Data;
            }

            public void Recycle()
            {
                if (process != null) Kill();

                Start();
            }

            public void Kill()
            {
                process.Kill();
                process.Close();
                process = null;
                startedErrorSampling = false;
                startedOutputSampling = false;
            }

        }

    }

    /// <summary>
    /// A reusable pattern to convert event into task
    /// http://stackoverflow.com/questions/22783741/a-reusable-pattern-to-convert-event-into-task
    /// </summary>
    public static class TaskExt
    {

        public static async Task<TEventArgs> FromEvent<TEventArgs>(
            Action<EventHandler<TEventArgs>> registerEvent,
            Action action,
            Action<EventHandler<TEventArgs>> unregisterEvent,
            CancellationToken token)
        {
            var tcs = new TaskCompletionSource<TEventArgs>();

            EventHandler<TEventArgs> handler = (sender, args) =>
                tcs.TrySetResult(args);

            registerEvent(handler);
            try
            {
                using (token.Register(() => tcs.SetCanceled()))
                {
                    action();
                    return await tcs.Task;
                }
            }
            finally
            {
                unregisterEvent(handler);
            }
        }

    }

}

