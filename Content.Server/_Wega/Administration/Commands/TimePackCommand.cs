using System.Linq;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Server.Player;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Permissions)]
public sealed class TimePackCommand : IConsoleCommand
{
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IConsoleHost _consoleHost = default!;

    public string Command => "timepack";
    public string Description => "Executes a set of commands based on the selected pack(s)";
    public string Help => "Usage: timepack <username> <pack1> [<pack2> ...]";

    private readonly Dictionary<int, List<string>> _packs = new()
    {
        { 1, new List<string>
            {
                "playtime_addoverall {0} 8400",
                "playtime_addrole {0} JobCaptain 1200",
                "playtime_addrole {0} JobStationEngineer 1200",
                "playtime_addrole {0} JobMedicalDoctor 1200",
                "playtime_addrole {0} JobSecurityOfficer 1200",
                "playtime_addrole {0} JobWarden 600",
                "playtime_addrole {0} JobAtmosphericTechnician 1201",
                "playtime_addrole {0} JobChemist 600",
                "playtime_addrole {0} JobScientist 900",
                "playtime_addrole {0} JobSalvageSpecialist 600",
                "playtime_addrole {0} JobServiceWorker 61"
            }
        },
        { 2, new List<string>
            {
                "playtime_addrole {0} JobSecurityOfficer 1200",
                "playtime_addrole {0} JobWarden 600",
                "playtime_addoverall {0} 600"
            }
        },
        { 3, new List<string>
            {
                "playtime_addrole {0} JobAtmosphericTechnician 600",
                "playtime_addrole {0} JobStationEngineer 1200"
            }
        },
        { 4, new List<string>
            {
                "playtime_addrole {0} JobChemist 600",
                "playtime_addrole {0} JobMedicalDoctor 1200"
            }
        },
        { 5, new List<string>
            {
                "playtime_addrole {0} JobScientist 900"
            }
        },
        { 6, new List<string>
            {
                "playtime_addrole {0} JobSalvageSpecialist 600",
                "playtime_addoverall {0} 2400"
            }
        },
        { 7, new List<string>
            {
                "playtime_addrole {0} JobBorg 900"
            }
        }
    };

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 2)
        {
            shell.WriteError("Invalid arguments. Usage: timepack <username> <pack1> [<pack2> ...]");
            return;
        }

        var username = args[0];
        var packIds = args.Skip(1).Select(arg =>
        {
            if (int.TryParse(arg, out var id))
                return id;
            return (int?)null;
        }).Where(id => id.HasValue).Select(id => id!.Value).ToList();

        if (packIds.Count == 0)
        {
            shell.WriteError("No valid packs provided.");
            return;
        }

        shell.WriteLine($"Received username: '{username}'");

        if (!_playerManager.TryGetSessionByUsername(username, out var playerSession))
        {
            shell.WriteError($"Session not found for '{username}'. Trying offline data...");

            var activePlayers = _playerManager.Sessions.Select(s => s.Name).ToList();
            shell.WriteLine($"Active players: {string.Join(", ", activePlayers)}");

            if (!_playerManager.TryGetPlayerDataByUsername(username, out var playerData))
            {
                shell.WriteError($"No player data found for '{username}'.");
                return;
            }

            foreach (var packId in packIds)
            {
                if (!_packs.TryGetValue(packId, out var commands))
                {
                    shell.WriteError($"Pack {packId} does not exist.");
                    continue;
                }

                foreach (var command in commands)
                {
                    var formattedCommand = string.Format(command, username);
                    _consoleHost.ExecuteCommand(formattedCommand);
                }
            }

            _consoleHost.ExecuteCommand($"playtime_save {username}");
            shell.WriteLine($"Executed timepack for user {username} (offline) with packs: {string.Join(", ", packIds)}.");
            return;
        }

        shell.WriteLine($"Session found for user {username} (online).");

        foreach (var packId in packIds)
        {
            if (!_packs.TryGetValue(packId, out var commands))
            {
                shell.WriteError($"Pack {packId} does not exist.");
                continue;
            }

            foreach (var command in commands)
            {
                var formattedCommand = string.Format(command, username);
                _consoleHost.ExecuteCommand(formattedCommand);
            }
        }

        _consoleHost.ExecuteCommand($"playtime_save {username}");
        shell.WriteLine($"Executed timepack for user {username} (online) with packs: {string.Join(", ", packIds)}.");
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHint("Enter username");

        if (args.Length > 1)
        {
            var hintOptions = _packs.Keys.Select(id => id.ToString()).ToList();
            return CompletionResult.FromHintOptions(hintOptions, "Enter pack number(s)");
        }

        return CompletionResult.Empty;
    }
}
