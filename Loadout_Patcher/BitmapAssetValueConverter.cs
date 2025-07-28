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
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Globalization;
using System.Reflection;

namespace Loadout_Patcher;

/// <summary>
/// <para>
/// Converts a string path to a bitmap asset.
/// </para>
/// <para>
/// The asset must be in the same assembly as the program. If it isn't,
/// specify "avares://<assemblynamehere>/" in front of the path to the asset.
/// </para>
/// </summary>
public class BitmapAssetValueConverter : IValueConverter
{
    //public static BitmapAssetValueConverter Instance = new BitmapAssetValueConverter();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            //return null;
            /* This must be checked in case of a custom map in Map.LoadoutMap since there is no picture */
            throw new Exception();
        }

        if (value is string rawUri && targetType.IsAssignableFrom(typeof(Bitmap)))
        {
            /*
            Uri uri;         
            
            // Allow for assembly overrides
            if (rawUri.StartsWith("avares://"))
            {
                uri = new Uri(rawUri);
            }
            else
            {
                string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
                uri = new Uri($"avares://{assemblyName}{rawUri}");
                //Uri rawestUri = new Uri(rawUri);
            }

            bool assetExists = AssetLoader.Exists(uri);
            (Stream stream, Assembly assembly) = AssetLoader.OpenAndGetAssembly(uri);
            // IEnumerable<Uri> assets = AssetLoader.GetAssets(uri, rawestUri);
            AssetLoader.Open(uri);

            //var asset = assets.FirstOrDefault().ToString();
            */

            // See "Display dynamic data"




            string assetPathNew = rawUri.Replace(@"/", ".");
            assetPathNew = assetPathNew.Replace(@"\", ".");
            assetPathNew = assetPathNew.Replace(@" ", "_");

            /* We use the namespace Loadout_Patcher */
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "Loadout_Patcher" + assetPathNew)!)
            {
                /*
                string check = System.AppDomain.CurrentDomain.BaseDirectory;
                //int index = check.IndexOf("Loadout_Patcher");
                int index = check.IndexOf(System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName));
                if (index >= 0)
                {
                    check = check.Substring(0, index) + System.IO.Path.GetFileNameWithoutExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName);
                }
                */
                //"/Assets/Maps/brewery_art_master.webp"
                // return new Bitmap(check + rawUri); 
                return new Bitmap(resourceStream);
            }
        }
        throw new NotSupportedException();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}