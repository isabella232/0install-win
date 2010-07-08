﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZeroInstall.Model;

namespace ZeroInstall.Publish.WinForms.FeedStructure
{
    public partial class ManifestDigestForm : Form
    {
        public ManifestDigestForm(ManifestDigest manifestDigest)
        {
            InitializeComponent();
            hintTextBoxSha1Old.Text = manifestDigest.Sha1Old;
            hintTextBoxSha1New.Text = manifestDigest.Sha1New;
            hintTextBoxSha256.Text = manifestDigest.Sha256;
        }
    }
}
