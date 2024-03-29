﻿using CommandLine;
using ONE;
using System;
using System.Threading.Tasks;

namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("roles", HelpText = "Retrieve Roles.")]
    public class RolesCommand: ICommand
    {
       
        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            var result = await clientSDK.Core.GetRolesAsync(false);
            if (result == null)
                return 0;
            else
            {
                foreach (var role in result)
                {
                    Console.WriteLine($"{role.Name}: {role.Id}");

                }
                return 1;
            }
        }
    }
}
