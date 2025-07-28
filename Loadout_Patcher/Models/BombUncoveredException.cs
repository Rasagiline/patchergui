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

namespace Loadout_Patcher.Models
{
    public class BombUncoveredException : Exception
    {
        public readonly CreationField Field;

        public BombUncoveredException(CreationField field)            
        {
            /* Don't catch this error! */
            Field = field;
        }
    }
}