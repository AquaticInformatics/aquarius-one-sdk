Logger APIs
====

 ..  class:: Logger 
    :module: ClientSDK

Events
-----
.. attribute:: Event
   :module: ClientSDK.Logger

   :rtype: EventHandler<ClientApiLoggerEventArgs>


Example WPF Logging UserControl
-----

C# Code-Behind
^^^^^^
.. code-block:: C#

   using ONE.UserControls.Utilities;
   using ONE.Utilities;
   using System;
   using System.Diagnostics;
   using System.Windows.Controls;
   using System.Windows.Navigation;

   namespace ONE.UserControls
   {
      /// <summary>
      /// Interaction logic for Logger.xaml
      /// </summary>
      public partial class Logger : UserControl
      {
         public Logger()
         {
               InitializeComponent();
         }
         public void Initialize(ClientSDK clientSDK)
         {
               clientSDK.Logger.Event += new EventHandler<ClientApiLoggerEventArgs>(SdkEvent);
         }
         void SdkEvent(object sender, ClientApiLoggerEventArgs e)
         {
               this.Dispatcher.Invoke(() =>
               {
                  lsvLogs.Items.Insert(0, new LogItem
                  {
                     Timestamp = DateTime.Now,
                     ElapsedMs = e.ElapsedMs,
                     Level = e.EventLevel,
                     Message = e.Message,
                     Module = e.Module,
                     HttpStatus = e.HttpStatusCode,
                     HttpCode = (int)e.HttpStatusCode,
                     File = e.File
                  });
               });
                  
         }

         private void OpenPageRequestNavigate(object sender, RequestNavigateEventArgs e)
         {
               // for .NET Core you need to add UseShellExecute = true
               // see https://docs.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
               Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
               e.Handled = true;
         }
      }
   }

User Control XAML
^^^^^^

.. code-block:: xml

   <UserControl x:Class="ONE.UserControls.Logger"
               xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
               xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
               xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
               xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
               xmlns:local="clr-namespace:ONE.UserControls"
               mc:Ignorable="d" 
               d:DesignHeight="450" d:DesignWidth="800">
      <Grid>
         <ListView Grid.Column="1" Grid.RowSpan="4" x:Name="lsvLogs" HorizontalAlignment="Stretch" HorizontalContentAlignment="Stretch">
               <ListView.View>
                  <GridView>
                     <GridViewColumn Header="Timestamp" DisplayMemberBinding="{Binding Timestamp}" Width="150"></GridViewColumn>
                     <GridViewColumn Header="ElapsedMs" DisplayMemberBinding="{Binding ElapsedMs}" Width="70"></GridViewColumn>
                     <GridViewColumn Header="HttpCode" DisplayMemberBinding="{Binding HttpCode}" Width="70"></GridViewColumn>
                     <GridViewColumn Header="HttpStatus" DisplayMemberBinding="{Binding HttpStatus}" Width="70"></GridViewColumn>
                     <GridViewColumn Header="Level" DisplayMemberBinding="{Binding Level}" Width="70"></GridViewColumn>
                     <GridViewColumn Header="Module" DisplayMemberBinding="{Binding Module}" Width="100"></GridViewColumn>
                     <GridViewColumn Header="Message" DisplayMemberBinding="{Binding Message}" Width="500"></GridViewColumn>
                     <GridViewColumn Header="Log"  Width="500">
                           <GridViewColumn.CellTemplate>
                              <DataTemplate>
                                 <TextBlock  Name="urlToContent" MinWidth="100" Width="Auto">
                                       <Hyperlink NavigateUri="{Binding Path=File}" Name="Log"   RequestNavigate="OpenPageRequestNavigate">
                                          <TextBlock Text="{Binding Path=File}"/>  
                                       </Hyperlink>
                                    </TextBlock>
                              </DataTemplate>
                           </GridViewColumn.CellTemplate>
                     </GridViewColumn>
                  </GridView>
               </ListView.View>
         </ListView>
      </Grid>
   </UserControl>

.. autosummary::
   :toctree: generated

  
