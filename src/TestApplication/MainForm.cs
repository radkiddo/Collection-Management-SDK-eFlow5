using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using TiS.Core.TisCommon;
using TiS.Core.Domain;
using TiS.Core.Common;
using TiS.Core.Application;
using TiS.Core.Application.Interfaces;
using TiS.Core.Application.DataModel.Dynamic;
using TiS.Core.Application.Workflow;

using eFlow.CollectionManagement;

namespace TestApplication
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnInstanceHistory_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Gets the history for collData

            string[] h = b.History(collData);

            MessageBox.Show("History for " + collData.Name + " is: " + String.Join("->", h));

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceRelease_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Release the collection "00000002"
            b.Hold("00000002");

            MessageBox.Show("Collection 00000002 on Hold - refresh the Controller");

            // Release the collection "00000002"
            b.Release("00000002");

            MessageBox.Show("Collection 00000002 released - refresh the Controller");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceReleaseMany_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Holds the collections "00000002" and "00000003"

            b.HoldMany(new string[] { "00000002", "00000003" });
            MessageBox.Show("Collections 00000002 and 00000003 on Hold - refresh the Controller");

            // Releases the collections "00000002" and "00000003"

            b.ReleaseMany(new string[] { "00000002", "00000003" });
            MessageBox.Show("Collections 00000002 and 00000003 released - refresh the Controller");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceMove_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Moves the collection "00000001" to
            // the "Export" station

            b.Move("00000001", "Export");
            MessageBox.Show("Collection 00000001 moved to Export  - refresh the Controller. Please, manually drag it back to FormId");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceMoveMany_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Moves the collections "00000002" and "00000003"
            // to the "Export" station

            b.MoveMany(new string[] { "00000001", "00000002" }, "Export");
            MessageBox.Show("Collections 00000001 and 00000002 moved to Export  - refresh the Controller. Please, manually drag it back to FormId");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceHold_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Release the collection "00000002"
            b.Hold("00000002");

            MessageBox.Show("Collection 00000002 on Hold - refresh the Controller");

            // Release the collection "00000002"
            b.Release("00000002");

            MessageBox.Show("Collection 00000002 released - refresh the Controller");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceHoldMany_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Holds the collections "00000002" and "00000003"

            b.HoldMany(new string[] { "00000002", "00000003" });
            MessageBox.Show("Collections 00000002 and 00000003 on Hold - refresh the Controller");

            // Releases the collections "00000002" and "00000003"

            b.ReleaseMany(new string[] { "00000002", "00000003" });
            MessageBox.Show("Collections 00000002 and 00000003 released - refresh the Controller");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstancePrioritize_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Sets the collection "00000002" to AboveNormal priority.

            b.Prioritize("00000002", Batch.Priority.AboveNormal);
            MessageBox.Show("Collection 00000002 AboveNormal priority - refresh the Controller");

            b.Prioritize("00000002", Batch.Priority.Normal);
            MessageBox.Show("Collection 00000002 Normal priority - refresh the Controller");


            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstancePrioritizeMany_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Prioritizes collections "00000002" and "00000003" with High priority

            b.PrioritizeMany(new string[] { "00000002", "00000003" }, Batch.Priority.High);
            MessageBox.Show("Collections 00000002 and 00000003 High priority - refresh the Controller");

            b.PrioritizeMany(new string[] { "00000002", "00000003" }, Batch.Priority.Normal);
            MessageBox.Show("Collections 00000002 and 00000003 Normal priority - refresh the Controller");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceFromDynamic_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Imports the collections present within the dynamic: "c:\temp\test.dynamic".

            ITisCollectionData[] collDatas = b.FromDynamic(@"c:\temp\test.dynamic");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceToDynamic_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Exports the collections "000002349_0001" and "000002349_0002" 
            // to the dynamic: "c:\temp\test.dynamic" file.

            b.ToDynamic(@"c:\temp\Dync2and3.dynamic", new string[] { "00000002", 
	            "00000002" });

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceLogout_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Do something here...
            MessageBox.Show("AppName is: " + b.ApplicationName);

            // Logs out fromn the CSM
            b.Logout();

            // Dispose the batch instance
            // and disconnect from the CSM
            // The Dispose() will not
            // log out from the CSM if the
            // Logout() has been called

            // If Logout() has not been called
            // then Dispose() will perform
            // the log out.

            // In any case Dispose() should always
            // be called. 

            b.Dispose();
        }

        private void btnInstanceLogin_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            // Use this line below if Login() is not used
            // Batch b = new Batch("AdvancedDemo", "FormId");

            // Otherwise use these next 4 lines:
            Batch b = new Batch();

            b.ApplicationName = "AdvancedDemo";
            b.StationName = "FormId";
            b.Login();

            // Do something here...
            MessageBox.Show("StationName is: " + b.StationName);

            // Logs out fromn the CSM
            b.Logout();

            // Dispose the batch instance
            // and disconnect from the CSM
            // The Dispose() will not
            // log out from the CSM if the
            // Logout() has been called

            // If Logout() has not been called
            // then Dispose() will perform
            // the log out.

            // In any case Dispose() should always
            // be called. 

            b.Dispose();
        }

        private void btnInstancePut_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Put the collection (writes it back)
            // to the CSM and moves it to the next
            // queue
            b.Put(collData);

            MessageBox.Show("Collection: " + collData.Name + " is now in the next queue (ManualId), please refresh the controller and drag it back");


            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceFree_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Put the collection (writes it back)
            // to the CSM and keeps it in the same
            // queue
            b.Free(collData);

            MessageBox.Show("Collection: " + collData.Name + " is still in the same queue");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Manually export collection 00000001 first through the controller, before proceeding");
            
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Deletes the collection
            // even if it is locked
            b.Delete(collData);

            MessageBox.Show("collection 00000001 is gone");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceReject_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Reject the collection
            b.Reject(collData);

            MessageBox.Show("collection 00000001 is in Reject, please refresh the controller and drag it back");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnInstanceGet_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station

            Batch b = new Batch("AdvancedDemo", "FormId");

            // Gets the collection "00000001"

            ITisCollectionData collData = b.Get("00000001");

            // Put the collection (writes it back)
            // to the CSM and keeps it in the same
            // queue
            b.Free(collData);

            MessageBox.Show("Collection: " + collData.Name + " is still in the same queue");

            // Dispose the batch instance
            // and disconnect from the CSM

            b.Dispose();
        }

        private void btnStaticHistory_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and gets the history of collection 
            // "00000001"

            string[] h = Batch.History("AdvancedDemo", "FormId", "00000001");

            MessageBox.Show("History for 00000001 is: " + String.Join("->", h));
        }

        private void btnStaticHistoryMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or Dispose()
            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and gets the history of collections "00000001", "00000002"
            // and "00000003"

            string[][] h = Batch.HistoryMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000002", "00000003" });

            if (h[0] != null && h[0].Length > 0) MessageBox.Show("History for 00000001 is: " + String.Join("->", h[0]));
            if (h[1] != null && h[1].Length > 0) MessageBox.Show("History for 00000002 is: " + String.Join("->", h[1]));
            if (h[2] != null && h[2].Length > 0) MessageBox.Show("History for 00000003 is: " + String.Join("->", h[1]));
        }

        private void btnStaticRelease_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and releases collection 
            // "00000002"

            Batch.Hold("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 on hold");

            Batch.Release("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 on released");
        }

        private void btnStaticReleaseMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or Dispose()
            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and releases collections "00000001"
            // and "00000003"

            Batch.HoldMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" });

            MessageBox.Show("00000001 and 00000003 on hold");

            Batch.ReleaseMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" });

            MessageBox.Show("00000001 and 00000003 released");
        }

        private void btnStaticMove_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and moves collection 
            // "00000002" to the "Export" station

            Batch.Move("AdvancedDemo", "FormId", "Export", "00000001");

            MessageBox.Show("00000001 on Export, please drag it back");
        }

        private void btnStaticMoveMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or Dispose()
            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and moves collections "00000001"
            // and "00000003" to the "Export" station

            Batch.MoveMany("AdvancedDemo", "FormId", "Export",
                new string[] { "00000001", "00000003" });

            MessageBox.Show("00000001 and 00000003 on Export, please drag them back");
        }

        private void btnStaticHold_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and releases collection 
            // "00000002"

            Batch.Hold("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 on hold");

            Batch.Release("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 on released");
        }

        private void btnStaticHoldMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or Dispose()
            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and releases collections "00000001"
            // and "00000003"

            Batch.HoldMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" });

            MessageBox.Show("00000001 and 00000003 on hold");

            Batch.ReleaseMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" });

            MessageBox.Show("00000001 and 00000003 released");
        }

        private void btnStaticPrioritize_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and sets the collection 
            // "00000002" on Low priority

            Batch.Prioritize("AdvancedDemo", "FormId", "00000002", Batch.Priority.Low);
            MessageBox.Show("00000002 with low priority");

            Batch.Prioritize("AdvancedDemo", "FormId", "00000002", Batch.Priority.Normal);
            MessageBox.Show("00000002 with normal priority");
        }

        private void btnStaticPrioritizeMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or Dispose()
            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and sets collections "00000001", "00000003" 
            // on High priority

            Batch.PrioritizeMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" }, Batch.Priority.High);

            MessageBox.Show("00000001 and 00000003 with high priority");

            Batch.PrioritizeMany("AdvancedDemo", "FormId",
                new string[] { "00000001", "00000003" }, Batch.Priority.Normal);

            MessageBox.Show("00000001 and 00000003 with normal priority");
        }

        private void btnStaticFromDynamic_Click(object sender, EventArgs e)
        {
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station
            // and imports the collections found within the
            // "c:\temp\test.dynamic" file

            string[] collDatas = Batch.FromDynamic("AdvancedDemo", "FormId", @"c:\temp\00000001.dynamic");
        }

        private void btnStaticToDynamic_Click(object sender, EventArgs e)
        {
            // Create a new Batch instance
            // Logs onto the CSM with "AdvancedDemo" application
            // and "FormId" station
            // and exports collections: "000002349_0001" and
            // "000002349_0002" to the "c:\temp\test.dynamic" file.

            Batch.ToDynamic("AdvancedDemo", "FormId", @"c:\temp\2and3.dynamic",
                new string[] { "00000002", "00000003" });
        }

        private void btnStaticDelete_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and deletes collection 
            // "00000002"

            Batch.Delete("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 deleted");
        }

        private void btnStaticDeleteMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and deletes collections 
            // "00000002" and "00000003"

            Batch.DeleteMany("AdvancedDemo", "FormId", new string[] { "00000002", "00000003" });
            MessageBox.Show("00000002 and 00000003 deleted");
        }

        private void btnStaticReject_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and rejects collection 
            // "00000002"

            Batch.Reject("AdvancedDemo", "FormId", "00000002");
            MessageBox.Show("00000002 rejected");
        }

        private void btnStaticRejectMany_Click(object sender, EventArgs e)
        {
            // Because this is an static method, it is atomic and does not require
            // the creation of an instance, or the use of Login(), Logout() or 
            // Dispose()

            // Logs onto the CSM with the "AdvancedDemo" application
            // and "FormId" station and rejects collections 
            // "00000002" and "00000003"

            Batch.RejectMany("AdvancedDemo", "FormId", new string[] { "00000002", "00000003" });
            MessageBox.Show("00000002 and 00000003 rejected");
        }

        private void btnInstanceUnlock_Click(object sender, EventArgs e)
        {
        }

        private void btnStaticUnlock_Click(object sender, EventArgs e)
        {
            Batch.UnlockSpecificCollection("00000001", "Data Source=EFLOW5-DMR-DEMO;Initial Catalog=AdvancedDemo_Workflow;Integrated Security=True");
        }
    }
}