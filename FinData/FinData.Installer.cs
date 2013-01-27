using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace FinData
{
//    1. Add a new class to the Primary Ouput project, derived from
//System.Configuration.Install.Installer and mark the class with the
//System.ComponentModel.RunInstaller attribute
//    2. Override the Install and Uninstall methods and use the
//System.Runtime.InteropServices.RegistrationServices class to register and
//unregister the assembly.
//    3. Add Custom Actions to the Install and Uninstall phases of the setup
//project. Make sure both actions have the InstallerClass property set to
//true.
//When the setup is executed, the Custom Actions will invoke the Installer
//derived class that you have added, and will do the registration /
//unregistration for you.

    [RunInstaller(true)]
    public class ComInstaller : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            System.Windows.Forms.MessageBox.Show("开始执行安装类自定义操作...");
            base.Install(stateSaver);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.RegisterAssembly(this.GetType().Assembly,AssemblyRegistrationFlags.SetCodeBase))
            {
                throw new InstallException("Failed To Register for COM");
            }
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            System.Windows.Forms.MessageBox.Show("开始执行uninstall类自定义操作...");

            base.Uninstall(savedState);
            RegistrationServices regsrv = new RegistrationServices();
            if (!regsrv.UnregisterAssembly(this.GetType().Assembly))
            {
                throw new InstallException("Failed To Unregister for COM");
            }
        }
    }
}

