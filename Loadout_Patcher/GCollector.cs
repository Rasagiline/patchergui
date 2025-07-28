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
    /// GCollector tries to keep the application efficient on PCs that are very busy with capacities on the limit
    /// </summary>
    public static class GCollector
    {
        // Don't enforce cleaning, not even if the program is idle

        /// <summary>
        /// Checks if the timing is right to clean up
        /// This is useful if the user's end device is very busy with other running processes, lacking available memory
        /// </summary>
        public static void ReportRightTimingForGc()
        {
            // 2: Generation 0 to 2 (all generations) are targeted
            // GCCollectionMode.Optimized: The GC will decide if it's the right timing
            // false: background collection requested
            // false: sweep only, no compacting
            GC.Collect(2, GCCollectionMode.Optimized, false, false);
        }
    }
}
