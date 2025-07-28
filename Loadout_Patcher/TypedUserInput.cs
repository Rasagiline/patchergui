/*******************************************************************************
 * Loadout_Patcher
 * 
 * Copyright (c) 2025 Rasagiline
 * GitHub: https://github.com/Rasagiline
 *
 * This program and the accompanying materials are made available under the
 * terms of the Eclipse Public License v. 2.0 which is available at
 * https://www.eclipse.org/legal/epl-2.0/
 *
 * SPDX-License-Identifier: EPL-2.0
 *******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loadout_Patcher
{
    /// <summary>
    /// TypedUserInput is for analyzing everything the user has typed in and sent
    /// </summary>
    public static class TypedUserInput
    {

        /// <summary>
        /// Assigns the new map with a map string typed by the user
        /// </summary>
        /// <param name="message"></param>
        /// <param name="map"></param>
        /// <param name="newMap"></param>
        public static void GetAndCheckUserString(string message, out string? map, out string newMap, bool isCustomMap = false)
        {
            while (true)
            {
                Console.WriteLine(message);
                /* Getting user input */
                map = Console.ReadLine();
                if (map != null)
                {
                    if (map.Length < 4)
                    {
                        Console.WriteLine("\n> Input is too short!\n");
                    }
                    else if (map.Length > 29)
                    {
                        Console.WriteLine("\n> Input is too long!\n");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            newMap = map;

            // TODO: Offer the user an upper case selection. They can choose with a checkbox click, type in manually or skip
            //// This will only be used if the GUI is ready
            //if (isCustomMap)
            //{
            //    Console.WriteLine("Please provide some details of the custom map you want to add:\n");

            //    /* Getting user input */
            //    Console.WriteLine("What is the map name in upper case?");
            //    string? fullMapNameAlt = Console.ReadLine(); // GUI: User gets a selection (guesses) but can also type it in
            //    Console.WriteLine("What is the base map called?");
            //    string? baseMap = Console.ReadLine(); // GUI: User clicks on one of many buttons
            //    Console.WriteLine("Is this a day or night map?");
            //    string? dayNight = Console.ReadLine(); // GUI: User clicks on one of 2 buttons
            //    Console.WriteLine("What is the game mode?");
            //    string? gameMode = Console.ReadLine(); // GUI: User clicks on one of many buttons

            //    if (baseMap != null && dayNight != null && gameMode != null && fullMapNameAlt != null)
            //    {
            //        Map.SetNewCustomMap(newMap, baseMap, dayNight, gameMode, fullMapNameAlt);
            //    }
            //}
        }

        /// <summary>
        /// Sets a new endpoint that must be used in order to communicate with the REST API
        /// </summary>
        /// <param name="defaultPatcherEndpoint"></param>
        /// <param name="newEndpoint"></param>
        public static void SetEndpoint(string defaultPatcherEndpoint, out string newEndpoint)
        {
            Console.WriteLine("> Default endpoint: " + defaultPatcherEndpoint + " (hit enter to choose that)\n");
            Console.Write("> Enter your new endpoint: ");

            /* Getting user input */
            string? readString = Console.ReadLine();

            if (readString == null)
            {
                readString = "";
            }
            newEndpoint = readString;

            /* In most cases, the empty check is all it needs because we expect the user to go for the offered endpoint */
            if (newEndpoint == "" || newEndpoint.Split('.').Length != 3  /* 3 = 2 dots */ || newEndpoint.Length < 4 || newEndpoint.Length > 50)
            {
                /* Invalid input gets an indicator that it becomes the default endpoint */
                if (!String.IsNullOrEmpty(newEndpoint)) { Console.WriteLine("                           is invalid and becomes\n                        -> "); }
                newEndpoint = defaultPatcherEndpoint;
                Console.WriteLine('\n' + newEndpoint);
            }
        }

        /// <summary>
        /// Waits for the user to type in a valid number as a selection and returns that number
        /// </summary>
        /// <param name="message"></param>
        /// <param name="maxSelection"></param>
        /// <param name="smallSelectionZeroDefault"></param>
        /// <returns>the option the user has chosen</returns>
        public static int GetUserNumberSelection(string message, int maxSelection, bool smallSelectionZeroDefault = false)
        {
            int option = -1;
            string invalidInputMessage = "\n> Invalid input. Please enter a correct option, a number from 0 to " + maxSelection + "!";

            /* We will not continue until we get a valid option. This is proven to be safe */
            while (true)
            {
                Console.Write("\n{0}", message);
                if (maxSelection > 10)
                {
                    string? stringMapIndexInput = Console.ReadLine();

                    /* Make sure the choice is valid because someone is bound to try and break it */
                    if (!String.IsNullOrEmpty(stringMapIndexInput) && stringMapIndexInput.Length < 3)
                    {
                        if (stringMapIndexInput.All(char.IsDigit)) {
                            int convertedMapIndexInput = int.Parse(stringMapIndexInput);
                            
                            if (convertedMapIndexInput <= maxSelection && convertedMapIndexInput >= 0)
                            {
                                option = convertedMapIndexInput;
                                return option;
                            }
                            else
                            {
                                Console.WriteLine(invalidInputMessage);
                            }

                        }
                        else
                        {
                            Console.WriteLine(invalidInputMessage);
                        }
                    }
                    else
                    {
                        Console.WriteLine(invalidInputMessage);
                    }
                }
                else
                {
                    char charMapIndexInput;

                    charMapIndexInput = Console.ReadKey(false).KeyChar;
                    Console.WriteLine();

                    /* Make sure the choice is valid because someone is bound to try and break it by entering the map name for example */
                    
                    int convertedMapIndexInput = charMapIndexInput - 48;
                    /* The input must be within the range of the selection or if it's allowed to be outside, the default option will be 0 */
                    if (convertedMapIndexInput <= maxSelection && convertedMapIndexInput >= 0 || smallSelectionZeroDefault)
                    {
                        if (convertedMapIndexInput > maxSelection || convertedMapIndexInput < 0)
                        {
                            option = 0;
                        }
                        else
                        {
                            option = convertedMapIndexInput;
                        }
                        return option;
                    }
                    Console.WriteLine(invalidInputMessage);

                }
            }
        }
    }
}
