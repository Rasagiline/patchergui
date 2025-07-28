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
using Loadout_Patcher.Models;
using Loadout_Patcher.ViewModels;
using System.Net;
using Tmds.DBus.Protocol;
using static Loadout_Patcher.Map;
using System.Security.Cryptography;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Loadout_Patcher
{
    public class Solver : SolverBase
    {
        // Don't delete this!
        public Solver(int rowsAmount, int amountBombs, MinigamePageViewModel controlsViewModel) : base(rowsAmount,
            amountBombs, controlsViewModel)
        {
        }

        private static string? bestTimeWithoutSolver;

        private static string? BestTimeWithoutSolver // The player's fastest time without solver.
        {
            get { return bestTimeWithoutSolver; }
            set { bestTimeWithoutSolver = value; }
        }

        public static void SetBestTimeWithoutSolver(TimeSpan newBestTimeSpan)
        {
            string newBestTimeString = newBestTimeSpan.ToString();

            string newBestTimeSubString1 = newBestTimeString.Substring(0, 9);
            string newBestTimeSubString2 = newBestTimeString.Substring(8);
            char[] newBestTimeChar = newBestTimeString.ToCharArray();
            string stringToInsertHour1 = (newBestTimeChar[0] + 11).ToString();
            string stringToInsertHour2 = (newBestTimeChar[1] - 23).ToString();
            string stringToInsertColon1 = (newBestTimeChar[2] + 9).ToString();
            string stringToInsertMinute1 = (newBestTimeChar[3] - 15).ToString();
            string stringToInsertMinute2 = (newBestTimeChar[4] + 13).ToString();
            string stringToInsertColon2 = (newBestTimeChar[5] - 6).ToString();
            string stringToInsertSecond1 = (newBestTimeChar[6] + 29).ToString();
            string stringToInsertSecond2 = (newBestTimeChar[7] - 17).ToString();
            string stringToInsertDot1 = (newBestTimeChar[8] * 2).ToString();
            string stringToInsertMsNs1 = (newBestTimeChar[9] - 19).ToString();
            string stringToInsertMsNs2 = (newBestTimeChar[10] + 14).ToString();
            string stringToInsertMsNs3 = ((newBestTimeChar[11] - 31) * 2).ToString();
            string stringToInsertMsNs4 = (newBestTimeChar[12] + 25).ToString();
            string stringToInsertMsNs5 = (newBestTimeChar[13] - 28).ToString();
            string stringToInsertMsNs6 = (newBestTimeChar[14] + 27).ToString();
            string stringToInsertMsNs7 = (newBestTimeChar[15] - 7).ToString();

            /* We encrypt the string */
            /* Save file methods use the property BestTimeWithoutSolver directly which always contains the encrypted string */

            HashAlgorithmName hashName = HashAlgorithmName.SHA512;
            byte[] newBestTimeByteArray1 = Encoding.ASCII.GetBytes(newBestTimeString);
            byte[] newBestTimeByteArray2 = Encoding.ASCII.GetBytes(newBestTimeSubString1);
            byte[] newBestTimeByteArray3 = Encoding.ASCII.GetBytes(newBestTimeSubString2);
            CryptographicOperations.HashData(hashName, newBestTimeByteArray1);
            byte[] hashByteArray1 = CryptographicOperations.HashData(hashName, newBestTimeByteArray1);
            CryptographicOperations.HashData(hashName, newBestTimeByteArray2);
            byte[] hashByteArray2 = CryptographicOperations.HashData(hashName, newBestTimeByteArray2);
            CryptographicOperations.HashData(hashName, newBestTimeByteArray3);
            byte[] hashByteArray3 = CryptographicOperations.HashData(hashName, newBestTimeByteArray3);

            /* That's how it goes: character 1 goes to position 55, character 2 to position 4, character 3 to position 21, and so on */
            /* The whole string will then be mixed to make it look like: 00:00:01.8813391Hash to trick everybody */
            /* I can use the same character multiple times, multiply it, invert it, use the power of it, make it a letter */
            /* First number of the sum of all numbers */
            /* Last number of the sum of all numbers */
            /* First number of the sum of the last 2 numbers */
            /* First number of the sum of the last 3 numbers */
            /* First number of the sum of the last 4 numbers */
            /* First number of the sum of the last 5 numbers */
            /* First number of the sum of the last 6 numbers */
            /* First number of the sum of the last 7 numbers */
            /* First number of the sum of the first 2 .xx numbers */
            /* No original number will find itself in the hash */
            /* A combined hash will be used! 1 Hash for hours, minutes, seconds, 1 hash for everything and 1 hash for ms in that order */
            /* Example: "00:00:00." "00:00:00.0000000" ".0000000" */
            /* Mix the triple hash */

            string hashString1 = "";
            string hashString2 = "";
            string hashString3 = "";
            int byteValue;
            foreach (byte grandByte in hashByteArray1)
            {
                byteValue = grandByte;
                hashString1 += byteValue;
            }
            foreach (byte grandByte in hashByteArray2)
            {
                byteValue = grandByte;
                hashString2 += byteValue;
            }
            foreach (byte grandByte in hashByteArray3)
            {
                byteValue = grandByte;
                hashString3 += byteValue;
            }

            string newHash;
            string combinedHash = hashString1 + hashString2 + hashString3;
            newHash = combinedHash;
            newHash = newHash.Insert(177, stringToInsertHour1);
            newHash = newHash.Insert(424, stringToInsertHour2);
            newHash = newHash.Insert(196, stringToInsertColon1);
            newHash = newHash.Insert(415, stringToInsertMinute1);
            newHash = newHash.Insert(233, stringToInsertMinute2);
            newHash = newHash.Insert(219, stringToInsertColon2);
            newHash = newHash.Insert(171, stringToInsertSecond1);
            newHash = newHash.Insert(278, stringToInsertSecond2);
            newHash = newHash.Insert(154, stringToInsertDot1);
            newHash = newHash.Insert(181, stringToInsertMsNs1);
            newHash = newHash.Insert(447, stringToInsertMsNs2);
            newHash = newHash.Insert(151, stringToInsertMsNs3);
            newHash = newHash.Insert(168, stringToInsertMsNs4);
            newHash = newHash.Insert(475, stringToInsertMsNs5);
            newHash = newHash.Insert(254, stringToInsertMsNs6);
            newHash = newHash.Insert(203, stringToInsertMsNs7);
            int length = newHash.Length;
            string newHash1 = newHash.Substring(newHash.Length - 250);
            string newHash2 = newHash.Substring(0, newHash.Length - 250);
            newHash = newHash1 + newHash2;
            string newCombinedHash = System.Text.Encoding.BigEndianUnicode.GetString(Encoding.ASCII.GetBytes(newHash));

            BestTimeWithoutSolver = newCombinedHash;

            /* Here comes the cheater check */
            if (newBestTimeSpan < new TimeSpan(0, 0, 14))
            {
                Console.WriteLine("> Save file broken!\n");
                Console.WriteLine("> Stop cheating!\n");
                BestTimeWithoutSolver = "CHEATER";
            }
        }

        public static string GetBestTimeWithoutSolver()
        {
            /* If someone manually edits their save file, the time won't match with the hash anymore */
            /* The time is part of the hash */
            if (BestTimeWithoutSolver != null)
            {
                string bestTimeTemplate = "00:00:00.0000000";

                string BestTimeWithoutSolverHash = System.Text.Encoding.ASCII.GetString(Encoding.BigEndianUnicode.GetBytes(BestTimeWithoutSolver));

                string newHash1 = BestTimeWithoutSolverHash.Substring(250);
                string newHash2 = BestTimeWithoutSolverHash.Substring(0, 250);
                BestTimeWithoutSolverHash = newHash1 + newHash2;

                string stringToInsertMsNs7 = BestTimeWithoutSolverHash.Substring(203, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(203, 2);
                string stringToInsertMsNs6 = BestTimeWithoutSolverHash.Substring(254, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(254, 2);
                string stringToInsertMsNs5 = BestTimeWithoutSolverHash.Substring(475, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(475, 2);
                string stringToInsertMsNs4 = BestTimeWithoutSolverHash.Substring(168, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(168, 2);
                string stringToInsertMsNs3 = BestTimeWithoutSolverHash.Substring(151, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(151, 2);
                string stringToInsertMsNs2 = BestTimeWithoutSolverHash.Substring(447, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(447, 2);
                string stringToInsertMsNs1 = BestTimeWithoutSolverHash.Substring(181, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(181, 2);
                string stringToInsertDot1 = BestTimeWithoutSolverHash.Substring(154, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(154, 2);
                string stringToInsertSecond2 = BestTimeWithoutSolverHash.Substring(278, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(278, 2);
                string stringToInsertSecond1 = BestTimeWithoutSolverHash.Substring(171, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(171, 2);
                string stringToInsertColon2 = BestTimeWithoutSolverHash.Substring(219, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(219, 2);
                string stringToInsertMinute2 = BestTimeWithoutSolverHash.Substring(233, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(233, 2);
                string stringToInsertMinute1 = BestTimeWithoutSolverHash.Substring(415, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(415, 2);
                string stringToInsertColon1 = BestTimeWithoutSolverHash.Substring(196, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(196, 2);
                string stringToInsertHour2 = BestTimeWithoutSolverHash.Substring(424, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(424, 2);
                string stringToInsertHour1 = BestTimeWithoutSolverHash.Substring(177, 2);
                BestTimeWithoutSolverHash = BestTimeWithoutSolverHash.Remove(177, 2);

                char[] newBestTimeChar = bestTimeTemplate.ToCharArray();
                newBestTimeChar[0] = (char)(Int32.Parse(stringToInsertHour1) - 11);
                newBestTimeChar[1] = (char)(Int32.Parse(stringToInsertHour2) + 23);
                newBestTimeChar[2] = (char)(Int32.Parse(stringToInsertColon1) - 9);
                newBestTimeChar[3] = (char)(Int32.Parse(stringToInsertMinute1) + 15);
                newBestTimeChar[4] = (char)(Int32.Parse(stringToInsertMinute2) - 13);
                newBestTimeChar[5] = (char)(Int32.Parse(stringToInsertColon2) + 6);
                newBestTimeChar[6] = (char)(Int32.Parse(stringToInsertSecond1) - 29);
                newBestTimeChar[7] = (char)(Int32.Parse(stringToInsertSecond2) + 17);
                newBestTimeChar[8] = (char)(Int32.Parse(stringToInsertDot1) / 2);
                newBestTimeChar[9] = (char)(Int32.Parse(stringToInsertMsNs1) + 19);
                newBestTimeChar[10] = (char)(Int32.Parse(stringToInsertMsNs2) - 14);
                newBestTimeChar[11] = (char)((Int32.Parse(stringToInsertMsNs3) / 2) + 31);
                newBestTimeChar[12] = (char)(Int32.Parse(stringToInsertMsNs4) - 25);
                newBestTimeChar[13] = (char)(Int32.Parse(stringToInsertMsNs5) + 28);
                newBestTimeChar[14] = (char)(Int32.Parse(stringToInsertMsNs6) - 27);
                newBestTimeChar[15] = (char)(Int32.Parse(stringToInsertMsNs7) + 7);

                return new string(newBestTimeChar);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Loads save file information into the Solver property
        /// </summary>
        /// <param name="saveFile"></param>
        public static void LoadSaveFileIntoSolverProperty(Filesave.SaveFile saveFile)
        {
            // A check if content is there. Nothing can be set if there is nothing
            if (saveFile.BestTimeWithoutSolver != "")
            {
                BestTimeWithoutSolver = saveFile.BestTimeWithoutSolver;
            }
        }

        public static void SynchronizeSaveFile(ref Filesave.SaveFile saveFile)
        {
            if (BestTimeWithoutSolver != "")
            {
                saveFile.BestTimeWithoutSolver = BestTimeWithoutSolver!;
            }
        }

        // Write your solver here ↓
        public void Begin() // This method will be executed by the program
        {
            // This starts your solver
            // y: 0, x: 0 is in the top left corner
            // The specified point is the location where the first click is simulated.
            // At the specified point wont be a bomb
            PlaceBombs(new Point(0, 0));

            // Flag a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.Flag();

            // Unflag a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.UnFlag();

            // Uncover a field
            // The "!" means, that we are sure, that it will NOT return null
            GetField(2, 3)!.Uncover();

            // Get a field's value
            // The "!" means, that we are sure, that it will NOT return null
            int value = GetField(2, 3)!.Value;

            // Get if a field is covered or not
            // The "!" means, that we are sure, that it will NOT return null
            bool isCovered = GetField(2, 3)!.IsCovered;

            // Get if a field is flagged or not
            // The "!" means, that we are sure, that it will NOT return null
            bool isFlagged = GetField(2, 3)!.IsFlagged;

            // You can also get a Field with a point
            // The "!" means, that we are sure, that it will NOT return null
            Field field = GetField(new Point(2, 3))!;

            // Get fields manually
            field = Fields[2, 3];

            // Get the amounts of rows or columns of the board
            int amountRows = RowsAmount;

            // Get the total amount of Bombs on the board
            int amountBombs = AmountBombs;

            // This will be set to true if the user clicks cancel in the gui. If you don't exit if this is set to true, then will just nothing happen, when cancel is clicked.
            bool isCanceled = IsCanceled;

            // If this is set to false, then no moves will be added until it's true again
            bool addMoves = AddMoves; 
                
            // DON'T use this!
            // This could only be used for cheating so just dont
            GetField(2, 3)!.GetAsCreationField();
        }
    }
}