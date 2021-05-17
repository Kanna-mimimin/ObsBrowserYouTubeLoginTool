using System;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Reactive.Linq;
using System.Reactive.Joins;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Binding;
using Reactive.Bindings.Interactivity;

namespace ObsBrowserYouTubeLoginTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public static string Title
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                return $"{assembly.Name} {assembly.Version.ToString(3)}";
            }
        }
        public ReactiveCommand<Models.CommandParameter> ExecuteCommand { get; } = new ReactiveCommand<Models.CommandParameter>();
        public ReactiveProperty<bool> IsDialogOpen { get; } = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> Executing { get; } = new ReactiveProperty<bool>();
        public ReactiveCommand OkCommand { get; } = new ReactiveCommand();
        public ReactiveCommand CancelCommand { get; } = new ReactiveCommand();
        public ReactiveProperty<bool> IsErrorDialogOpen { get; } = new ReactiveProperty<bool>();
        public ReactiveCommand ErrorOkCommand { get; } = new ReactiveCommand();

        public Models.CommandParameter Param;
        public ReactiveCommand<Models.CommandParameter> EdgeCommand { get; }
        public ReactiveCommand<Models.CommandParameter> ChromeCommand { get; }
        public ReactiveProperty<string> EdgeLocalStateFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.EdgeLocalStateFilePath);
        public ReactiveProperty<string> EdgeCookiesFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.EdgeCookiesFilePath);        
        public ReactiveProperty<string> ChromeLocalStateFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.ChromeLocalStateFilePath);
        public ReactiveProperty<string> ChromeCookiesFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.ChromeCookiesFilePath);
        public ReactiveProperty<string> ObsCookiesFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.ObsCookiesFilePath);

        public ReactiveCommand<Models.CommandParameter> ParamCommand { get; }
        public ReactiveProperty<string> LocalStateFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.EdgeLocalStateFilePath);
        public ReactiveProperty<string> SrcCookieFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.EdgeCookiesFilePath);
        public ReactiveProperty<string> HostKey { get; } = new ReactiveProperty<string>(Properties.Settings.Default.HostKey);
        public ReactiveProperty<string> DestCookieFilePath { get; } = new ReactiveProperty<string>(Properties.Settings.Default.ObsCookiesFilePath);



        public MainWindowViewModel()
        {
            EdgeLocalStateFilePath.SetValidateNotifyError(IsExitisFile);
            EdgeCookiesFilePath.SetValidateNotifyError(IsExitisFile);
            ChromeLocalStateFilePath.SetValidateNotifyError(IsExitisFile);
            ChromeCookiesFilePath.SetValidateNotifyError(IsExitisFile);
            ObsCookiesFilePath.SetValidateNotifyError(IsExitisFile);
            EdgeCommand = new[] { EdgeLocalStateFilePath.ObserveHasErrors, EdgeCookiesFilePath.ObserveHasErrors, ObsCookiesFilePath.ObserveHasErrors }.CombineLatestValuesAreAllFalse().ToReactiveCommand<Models.CommandParameter>();
            ChromeCommand = new[] { ChromeLocalStateFilePath.ObserveHasErrors, ChromeCookiesFilePath.ObserveHasErrors, ObsCookiesFilePath.ObserveHasErrors }.CombineLatestValuesAreAllFalse().ToReactiveCommand<Models.CommandParameter>();

            LocalStateFilePath.SetValidateNotifyError(IsExitisFile);
            SrcCookieFilePath.SetValidateNotifyError(IsExitisFile);
            DestCookieFilePath.SetValidateNotifyError(IsExitisFile);
            HostKey.SetValidateNotifyError(host =>
            {
                if (String.IsNullOrWhiteSpace(host))
                {
                    return "Empty";
                }

                return null;
            });

            ParamCommand = new[] { LocalStateFilePath.ObserveHasErrors, SrcCookieFilePath.ObserveHasErrors, HostKey.ObserveHasErrors, DestCookieFilePath.ObserveHasErrors }.CombineLatestValuesAreAllFalse().ToReactiveCommand<Models.CommandParameter>();

            EdgeCommand.Subscribe(ExecuteCommand.Execute);
            ChromeCommand.Subscribe(ExecuteCommand.Execute);
            ParamCommand.Subscribe(ExecuteCommand.Execute);

            OkCommand.Subscribe(async _ =>
            {
                Executing.Value = true;

                try
                {
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        CookieManager.Converter.Convert(Environment.ExpandEnvironmentVariables(Param.LocalStateFilePath), Environment.ExpandEnvironmentVariables(Param.SrcCookieFilePath), Param.HostKey, Environment.ExpandEnvironmentVariables(Param.DestCookieFilePath));
                    });
                }
                catch
                {
                    IsErrorDialogOpen.Value = true;
                }

                Executing.Value = false;
                IsDialogOpen.Value = false;
            }
            );

            ExecuteCommand.Subscribe(param =>
            {
                IsDialogOpen.Value = true;
                Param = param;
            });

            CancelCommand.Subscribe(_ => IsDialogOpen.Value = false);

            ErrorOkCommand.Subscribe(_ => IsErrorDialogOpen.Value = false);
        }

        private static string IsExitisFile(string path)
        {
            if (!System.IO.File.Exists(Environment.ExpandEnvironmentVariables(path)))
            {
                return "NotFound";
            }
            return null;
        }
    }
}
