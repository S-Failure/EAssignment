﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EAssignment.Security
{
    public class CustomEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
        public CustomEmailConfirmationTokenProviderOptions()
        {

        }
    }
}
