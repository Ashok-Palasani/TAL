using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static UnitWorksCCS.FTPLibrary;

namespace UnitWorksCCS
{
    class FtpMain
    {
        public FTPclient FtpClient { get; private set; }


        #region Functions
        /// <summary>
        /// Sets up the FTPClient for this Form.  Called from frmLogin.
        /// </summary>
        /// <param name="client">FTPclient on frmLogin is used to refrence the FtpClient here.</param>
        //public void SetFtpClient(FTPclient client)
        public List<ProgramListDet> SetFtpClient(FTPclient client)
        {
            //Set FtpClient
            FtpClient = client;
            List<ProgramListDet> objList = new List<ProgramListDet>();

            //Display the Welcome Message
            //Message = new ListViewItem();
            //Message.Text = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
            //Message.SubItems.Add("Welcome Message");
            //Message.SubItems.Add(FtpClient.WelcomeMessage);
            //Message.SubItems.Add("No Code");
            //Message.SubItems.Add("/");
            //lstMessages.Items.Add(Message);

            //Setup OnMessageReceived Event
            FtpClient.OnNewMessageReceived += new FTPclient.NewMessageHandler(FtpClient_OnNewMessageReceived);

            //Open and Display Root Directory and Files/Folders in it
            foreach (FTPfileInfo folder in FtpClient.ListDirectoryDetail("/"))
            {
                ListViewItem item = new ListViewItem();
                item.Text = folder.Filename;
                if (folder.FileType == FTPfileInfo.DirectoryEntryTypes.Directory)
                    item.SubItems.Add("Folder");
                else
                    item.SubItems.Add("File");

                item.SubItems.Add(folder.Filename);
                item.SubItems.Add(folder.Permission);
                item.SubItems.Add(folder.FileDateTime.ToShortTimeString() + folder.FileDateTime.ToShortDateString());
                item.SubItems.Add(GetFileSize(folder.Size));

                ProgramListDet obj = new ProgramListDet();
                obj.ProgName = folder.Filename;
                obj.ProgDate = folder.FileDateTime.ToString("yyyy-MM-dd");
                obj.ProgNo = folder.Filename;
                obj.ProgSize = Convert.ToString(folder.Size);
                objList.Add(obj);
                //lstRemoteSiteFiles.Items.Add(item);
            }
            return objList;
        }

        /// <summary>
        /// Reload all Directories and Files in Current Directory
        /// </summary>
        //private void RefreshDirectory()
        //{
        //    //Clear all items
        //    //lstRemoteSiteFiles.Items.Clear();

        //    //Open and Display Root Directory
        //    foreach (FTPfileInfo folder in FtpClient.ListDirectoryDetail(txtRemoteDirectory.Text))
        //    {
        //        ListViewItem item = new ListViewItem();
        //        item.Text = folder.Filename;
        //        if (folder.FileType == FTPfileInfo.DirectoryEntryTypes.Directory)
        //            item.SubItems.Add("Folder");
        //        else
        //            item.SubItems.Add("File");

        //        item.SubItems.Add(folder.FullName);
        //        item.SubItems.Add(folder.Permission);
        //        item.SubItems.Add(folder.FileDateTime.ToShortTimeString() + folder.FileDateTime.ToShortDateString());
        //        item.SubItems.Add(folder.Size.ToString());
        //        //lstRemoteSiteFiles.Items.Add(item);
        //    }
        //}

        #region Download File
        /// <summary>
        /// Download File: When the Download ToolStripButton is clicked. Displays a SaveFileDialog.
        /// </summary>
        /// <param name="FileName">Name of the File to Download</param>
        /// <param name="CurrentDirectory">CurrentDirectory (Directory from which to download on server)</param>
        private void DownloadFile(string FileName, string CurrentDirectory)
        {
            //Setup and Show 
            SaveFileDialog SaveDialog = new SaveFileDialog();
            SaveDialog.FileName = FileName;
            SaveDialog.Title = "Save File to...";
            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                //Setup and Open Download Form
                //frmDownload DownloadForm = new frmDownload(FileName, CurrentDirectory, SaveDialog.FileName, FtpClient);
            }
            else
                MessageBox.Show("Download has been cancelled.");        //Notify user that Download has been cancelled.
        }

        /// <summary>
        /// Download File: When File is dragged from ListView to the tvFileSystem.  No SaveAsDialog/SavePath is the selected node in
        /// tvFileSystem
        /// </summary>
        /// <param name="FileName">Name of File to Download</param>
        /// <param name="CurrentDirectory">CurrentDirectory (Directory from which to download on server)</param>
        /// <param name="SavePath">Path where file will be downloaded.</param>
        private void DownloadFile(string FileName, string CurrentDirectory, string SavePath)
        {
            //Setup and Open Download Form
            if (SavePath.EndsWith("\\"))
            {
                //frmDownload DownloadForm = new frmDownload(FileName, CurrentDirectory, SavePath + FileName, FtpClient);
            }
            else
            {
                ////Setup and Open Download Form
                //frmDownload DownloadForm = new frmDownload(FileName, CurrentDirectory, SavePath + "\\" + FileName, FtpClient);
            }
        }
        #endregion

        //Code Below Converts Bytes to KB, MB, GB, or just Bytes.  Makes the App more look :)
        //Obtained from: http://www.freevbcode.com/ShowCode.Asp?ID=1971
        private string GetFileSize(double byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }
        #endregion

        #region Events
        private void btnGo_Click(object sender, EventArgs e)
        {
            try
            {
                // navigate to specific folder
                //DirNode = new TreeNode();
                //tvFileSystem.ShowFolder(txtLocalFolderName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //private void btnOpenDialog_Click(object sender, EventArgs e)
        //{
        //    //Setup the Open Dialog
        //    FolderBrowserDialog objFolderDialog = new FolderBrowserDialog();
        //    objFolderDialog.ShowNewFolderButton = true;
        //    objFolderDialog.Description = "Select Folder to view in TreeView.";

        //    //Show Dialog
        //    if (objFolderDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        txtLocalFolderName.Text = objFolderDialog.SelectedPath;
        //        try
        //        {
        //            // navigate to specific folder on tvFileSystem
        //            DirNode = new TreeNode();
        //            tvFileSystem.ShowFolder(txtLocalFolderName.Text);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Display Error
        //            MessageBox.Show("Error: " + ex.Message);
        //        }
        //    }
        //}

        //private void frmMain_Resize(object sender, EventArgs e)
        //{
        //    //Resizing the controls will keep the form looking good and neat.
        //    //Keep both GroupBoxes even on the Form
        //    gbFileSystem.Size = new Size(this.Size.Width / 2, gbFileSystem.Height);
        //    gbRemoteSite.Size = gbFileSystem.Size;

        //    //Resize txtFolderName so it doesn't get hidden if form is too small
        //    txtLocalFolderName.Size = new Size(toolStrip1.Size.Width - 115, txtLocalFolderName.Size.Height);
        //    txtRemoteDirectory.Size = txtLocalFolderName.Size;
        //}

        //private void frmMain_Load(object sender, EventArgs e)
        //{
        //    //Keep both GroupBoxes even on the Form
        //    gbFileSystem.Size = new Size(this.Size.Width / 2, gbFileSystem.Height);
        //    gbRemoteSite.Size = gbFileSystem.Size;
        //    tvFileSystem.AllowDrop = true;
        //}

        private void FtpClient_OnNewMessageReceived(object myObject, NewMessageEventArgs e)
        {

            string res = e.StatusType + e.StatusMessage + e.StatusCode;
            //Display Meesage in lstMessages
            //Message = new ListViewItem();
            //Message.Text = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
            //Message.SubItems.Add(e.StatusType);
            //Message.SubItems.Add(e.StatusMessage);
            //Message.SubItems.Add(e.StatusCode);
            //Message.SubItems.Add(txtRemoteDirectory.Text);
            //lstMessages.Items.Add(Message);

            //this.lstMessages.EnsureVisible(this.lstMessages.Items.Count - 1);
        }

        ////Fires when the Selected File/Folder is changed in lstRemoteFiles.
        //private void lstRemoteSiteFiles_SelectedValueChanged(object sender, EventArgs e)
        //{
        //    //Check if there is a file; if there isn't select the first one.
        //    //All this comes under Try,Catch.  In case, there isn't a file or something.
        //    try
        //    {
        //        if (lstRemoteSiteFiles.SelectedItems[0] == null)
        //        {
        //            lstRemoteSiteFiles.Items[0].Selected = true;
        //        }
        //    }
        //    catch
        //    {
        //        return;
        //    }

        //    //If the Selected Item is a File, then we want its related buttons to be enables
        //    if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "File")
        //    {
        //        //Enable the buttons that have to do with the FILE
        //        btnRename.Enabled = true;
        //        btnDownload.Enabled = true;
        //    }
        //    else if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "Folder") // Its a Directory, Disable buttons
        //    {
        //        //Disable the buttons that have nothing to do with the FOLDER
        //        btnRename.Enabled = false;
        //        btnDownload.Enabled = false;
        //    }
        //}

        //private void btnRemoteBack_Click(object sender, EventArgs e)
        //{
        //    // Locate the Last "/"
        //    if (txtRemoteDirectory.Text != "/")
        //    {
        //        //The below code works fine, even though it repeats.  I'll explain why it repeats:
        //        //1) If we have "/Dire1/Drect2/" we first take out the extra "/" and then we do it again to remove the other directory name
        //        //2) I didn't really test much of it...after I got it to work
        //        //3) If you can get it to work in a more efficient way than below, then ur most welcome to fix it!
        //        int endTagStartPosition = txtRemoteDirectory.Text.LastIndexOf("/");
        //        txtRemoteDirectory.Text = txtRemoteDirectory.Text.Substring(0, endTagStartPosition);
        //        int endTagStartPosition1 = txtRemoteDirectory.Text.LastIndexOf("/");
        //        txtRemoteDirectory.Text = txtRemoteDirectory.Text.Substring(0, endTagStartPosition1);

        //        //If there is "/" that means that we are at root and we don't need "//",
        //        //if not, then we add "/" at the end of the directory, so our above code
        //        //works w/o errors
        //        if (txtRemoteDirectory.Text != "/")
        //            txtRemoteDirectory.Text += "/";

        //        //Empty lstRemoteFiles
        //        lstRemoteSiteFiles.Items.Clear();
        //        //Set Current Directory
        //        FtpClient.CurrentDirectory = txtRemoteDirectory.Text;

        //        //Get Files and Folders from Selected Direcotry
        //        foreach (FTPfileInfo folder in FtpClient.ListDirectoryDetail(txtRemoteDirectory.Text))
        //        {
        //            ListViewItem item = new ListViewItem();
        //            item.Text = folder.Filename;
        //            if (folder.FileType == FTPfileInfo.DirectoryEntryTypes.Directory)
        //                item.SubItems.Add("Folder");
        //            else
        //                item.SubItems.Add("File");

        //            item.SubItems.Add(folder.FullName);
        //            item.SubItems.Add(folder.Permission);
        //            item.SubItems.Add(folder.FileDateTime.ToShortTimeString() + folder.FileDateTime.ToShortDateString());
        //            item.SubItems.Add(folder.Size.ToString());
        //            lstRemoteSiteFiles.Items.Add(item);
        //        }
        //    }
        //}

        //private void txtLocalFolderName_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        try
        //        {
        //            // navigate to specific folder
        //            DirNode = new TreeNode();
        //            tvFileSystem.ShowFolder(txtLocalFolderName.Text);
        //            //PopulateTree(txtLocalFolderName.Text, DirNode);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error: " + ex.Message);
        //        }
        //    }
        //}

        //private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "File")
        //        {
        //            string extension = lstRemoteSiteFiles.SelectedItems[0].Text.Substring(lstRemoteSiteFiles.SelectedItems[0].Text.LastIndexOf("."));
        //            //Create a RenameDialog and display it.
        //            RenameDialog = new frmRename(lstRemoteSiteFiles.SelectedItems[0].Text, txtRemoteDirectory.Text, this, extension);
        //            RenameDialog.ShowDialog(this);

        //        }
        //        else
        //            RenameDialog = new frmRename(lstRemoteSiteFiles.SelectedItems[0].Text, txtRemoteDirectory.Text, this, "");
        //        //Refresh, because the Filename has been changed by the user.
        //        RefreshDirectory();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (lstRemoteSiteFiles.SelectedItems[0] != null)
        //    {
        //        //Check if File or Folder
        //        if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "File")
        //        {
        //            try
        //            {
        //                //Delete the FILE
        //                FtpClient.FtpDelete(lstRemoteSiteFiles.SelectedItems[0].Text);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Error Occured while Deleting File.  Error Message: " + ex.Message);
        //            }
        //        }
        //        else
        //        {
        //            try
        //            {
        //                //Delete the FOLDER
        //                FtpClient.FtpDeleteDirectory(lstRemoteSiteFiles.SelectedItems[0].SubItems[0].Text);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Error Occured while Deleting File.  Error Message: " + ex.Message);
        //            }
        //        }

        //        RefreshDirectory();
        //    }
        //}

        //private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (lstRemoteSiteFiles.SelectedItems[0] != null)
        //        {
        //            if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "File")
        //            {
        //                //Download File
        //                DownloadFile(lstRemoteSiteFiles.SelectedItems[0].Text, FtpClient.CurrentDirectory);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        //private void btnNewDir_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //New instance of frmNewFolder
        //        frmNewFolder NewFolderForm = new frmNewFolder();
        //        if (NewFolderForm.ShowDialog() == DialogResult.OK)
        //        {
        //            //Create New Directory
        //            FtpClient.FtpCreateDirectory(FtpClient.CurrentDirectory + NewFolderForm.NewDirName);
        //            //Refresh Current Directory to view the newly created directory
        //            RefreshDirectory();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error Occured while Creating Folder. Error Message: " + ex.Message);
        //    }
        //}

        #region Drag & Drop to FileSystem from Remote Server
        //private void lstRemoteSiteFiles_ItemDrag(object sender, ItemDragEventArgs e)
        //{
        //    lstRemoteSiteFiles.DoDragDrop(e.Item, DragDropEffects.Copy);
        //}

        private void tvFileSystem_DragEnter(object sender, DragEventArgs e)
        {
            // this code can be in DragOver also
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                ListViewItem li = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                //Allow a FIle to be compied, no folders.
                if (li.SubItems[1].Text == "File")
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }

        //private void tvFileSystem_DragDrop(object sender, DragEventArgs e)
        //{
        //    if (e.Data.GetDataPresent(typeof(ListViewItem)))
        //    {
        //        ListViewItem li = (ListViewItem)e.Data.GetData(typeof(ListViewItem));

        //        try
        //        {
        //            //Double Check if its a file....Just in case :)
        //            if (li.SubItems[1].Text == "File")
        //            {
        //                Point pos = tvFileSystem.PointToClient(new Point(e.X, e.Y));
        //                TreeNode targetNode = tvFileSystem.GetNodeAt(pos);
        //                if (targetNode != null)
        //                {
        //                    //Set SelectedNode
        //                    TreeNodePath SelectedNode = targetNode as TreeNodePath;
        //                    //Ask User if he/she wants to save
        //                    DialogResult DownloadConfirm = MessageBox.Show("Do you want to save " + li.Text + " to " + SelectedNode.Path + "?", "Download File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //                    //DialogResult
        //                    if (DownloadConfirm == DialogResult.Yes)
        //                    {
        //                        //Download File if DownloadResult = YES
        //                        DownloadFile(li.Text, FtpClient.CurrentDirectory, SelectedNode.Path);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //    }
        //}

        //private void tvFileSystem_DragOver(object sender, DragEventArgs e)
        //{
        //    // this code can be in DragOver also
        //    if (e.Data.GetDataPresent(typeof(ListViewItem)))
        //    {
        //        ListViewItem li = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
        //        if (li.SubItems[1].Text == "File")
        //        {
        //            e.Effect = DragDropEffects.Copy;
        //        }
        //        else
        //            e.Effect = DragDropEffects.None;

        //    }
        //}
        #endregion

        //private void lstRemoteSiteFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    if (lstRemoteSiteFiles.Items.Count != 0)
        //    {
        //        try
        //        {
        //            if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "File")
        //            {
        //                //Enable the buttons that are related to the FILE
        //                btnRename.Enabled = true;
        //                btnDownload.Enabled = true;
        //                //Set Current Directory for Download
        //                FtpClient.CurrentDirectory = txtRemoteDirectory.Text;
        //                //Its a File, so Ask them if they want to Save it...
        //                if (MessageBox.Show("Do you want to save this file: " + txtRemoteDirectory.Text + lstRemoteSiteFiles.SelectedItems[0].Text + "/" + "?", "Download File?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
        //                {
        //                    //Save the File to location
        //                    downloadToolStripMenuItem_Click(this, e);
        //                }
        //            }
        //            else if (lstRemoteSiteFiles.SelectedItems[0].SubItems[1].Text == "Folder") // Its a Directory
        //            {
        //                //Set Directory to txtRemoteDirectory.Text + selectedItem + "/"
        //                //Result - /SelectedDirecotory/  -- good for navigation, keeping user informed and code :)
        //                txtRemoteDirectory.Text += lstRemoteSiteFiles.SelectedItems[0].Text + "/";
        //                lstRemoteSiteFiles.Items.Clear();

        //                //Set Current Dir
        //                FtpClient.CurrentDirectory = txtRemoteDirectory.Text;

        //                //Get Files and Folders from Selected Direcotry
        //                foreach (FTPfileInfo folder in FtpClient.ListDirectoryDetail(txtRemoteDirectory.Text))
        //                {
        //                    ListViewItem item = new ListViewItem();
        //                    item.Text = folder.Filename;
        //                    if (folder.FileType == FTPfileInfo.DirectoryEntryTypes.Directory)
        //                        item.SubItems.Add("Folder");
        //                    else
        //                        item.SubItems.Add("File");

        //                    item.SubItems.Add(folder.FullName);
        //                    item.SubItems.Add(folder.Permission);
        //                    item.SubItems.Add(folder.FileDateTime.ToShortTimeString() + folder.FileDateTime.ToShortDateString());
        //                    item.SubItems.Add(GetFileSize(folder.Size));
        //                    lstRemoteSiteFiles.Items.Add(item);
        //                }
        //            }
        //        }
        //        catch { }
        //    }
        //}

        //private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    //Display the Hidden frmLogin form
        //    Application.OpenForms[0].Show();
        //}

        //private void btnUpload_Click(object sender, EventArgs e)
        //{
        //    objOpenDialog = new OpenFileDialog();
        //    objOpenDialog.Multiselect = false;
        //    objOpenDialog.Filter = "All files (*.*)|*.*";
        //    objOpenDialog.RestoreDirectory = true;
        //    if (objOpenDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        //Declare and Setup out UploadForm Variable.  The frmUpload Constructor will do everything else, including showing the form.
        //        frmUpload UploadForm = new frmUpload(objOpenDialog.FileName, FtpClient.CurrentDirectory, FtpClient);
        //    }
        //    //Call RefreshDirectory; Refresh the Files and Folders for Current Directory.
        //    RefreshDirectory();
        //}
        #endregion        
    }

}
