using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Parse;
namespace Tennis
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App(){
            this.InitializeComponent();

            ParseClient.Initialize("trBz9Oyi6HXaAFZ2FINuAScTBulfxbIxXqtodyxE", "ptV7LDRm5SVnuaKgUMMo3DGSXCjd0A7TOddLmWnj");
        }
       
    }
}
