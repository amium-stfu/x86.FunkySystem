using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FunkySystem.Net.CAN
{
    using TPCANHandle = System.UInt16;
    using TPCANBitrateFD = System.String;
    using TPCANTimestampFD = System.UInt64;

    public class CanPeak
    {
        private delegate void ReadDelegateHandler();

        public CanPeak()
        {
        }

        public List<TPCANMsg> Receive()
        {
            List<TPCANMsg> messages = new List<TPCANMsg>();
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsg, out CANTimeStamp);

            while ((stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                && (stsResult != TPCANStatus.PCAN_ERROR_INITIALIZE))
            
                {
                /*
                CanMessage cm = new CanMessage();
                cm.Id = CANMsg.ID;
                cm.Dlc = CANMsg.LEN;
                for (int i = 0; i< CANMsg.LEN; i++)
                {
                    cm.Bytes[i] = CANMsg.DATA[i];
                }

                cm.Date = DateTime.Now;*/
                messages.Add(CANMsg);
           //     Can.ReceiveCallback(cm);



                stsResult = PCANBasic.Read(m_PcanHandle, out CANMsg, out CANTimeStamp);
                // We process the received message
                //
                //   ProcessMessage(CANMsg, CANTimeStamp);
            }
            return messages;
           // return stsResult;

           // ReadMessage();
              //  return;

            if (m_ReceiveEvent.WaitOne(50))
            {
                ReadMessages();
            }
                // Process Receive-Event using .NET Invoke function
                // in order to interact with Winforms UI (calling the 
                // function ReadMessages)
                // 
             //   (m_ReadDelegate);
        }

        private string FormatChannelName(TPCANHandle handle, bool isFD)
        {
            TPCANDevice devDevice;
            byte byChannel;

            // Gets the owner device and channel for a 
            // PCAN-Basic handle
            //
            if (handle < 0x100)
            {
                devDevice = (TPCANDevice)(handle >> 4);
                byChannel = (byte)(handle & 0xF);
            }
            else
            {
                devDevice = (TPCANDevice)(handle >> 8);
                byChannel = (byte)(handle & 0xFF);
            }

            // Constructs the PCAN-Basic Channel name and return it
            //
            if (isFD)
                return string.Format("{0}:FD {1} ({2:X2}h)", devDevice, byChannel, handle);
            else
                return string.Format("{0} {1} ({2:X2}h)", devDevice, byChannel, handle);
        }

        private string FormatChannelName(TPCANHandle handle)
        {
            return FormatChannelName(handle, false);
        }

        System.Windows.Forms.ComboBox cbbChannel = new System.Windows.Forms.ComboBox();

        private void btnHwRefresh_Click(object sender, EventArgs e)
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;
            bool isFD;

            // Clears the Channel combioBox and fill it again with 
            // the PCAN-Basic handles for no-Plug&Play hardware and
            // the detected Plug&Play hardware
            //
            cbbChannel.Items.Clear();
            try
            {
                for (int i = 0; i < m_HandlesArray.Length; i++)
                {
                    // Includes all no-Plug&Play Handles
                    if (m_HandlesArray[i] <= PCANBasic.PCAN_DNGBUS1)
                        cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i]));
                    else
                    {
                        // Checks for a Plug&Play Handle and, according with the return value, includes it
                        // into the list of available hardware channels.
                        //
                        stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_CONDITION, out iBuffer, sizeof(UInt32));
                        if ((stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE))
                        {
                            stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_FEATURES, out iBuffer, sizeof(UInt32));
                            isFD = (stsResult == TPCANStatus.PCAN_ERROR_OK) && ((iBuffer & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE);
                            cbbChannel.Items.Add(FormatChannelName(m_HandlesArray[i], isFD));
                        }
                    }
                }
                cbbChannel.SelectedIndex = cbbChannel.Items.Count - 1;
                Enabled = cbbChannel.Items.Count > 0;
            }
            catch (DllNotFoundException)
            {
        //        MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        Environment.Exit(-1);
            }
        }

        private void cbbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bNonPnP;
            string strTemp;

            // Get the handle fromt he text being shown
            //
            strTemp = cbbChannel.Text;

            if (string.IsNullOrEmpty(strTemp))
                return;

            strTemp = strTemp.Substring(strTemp.IndexOf('(') + 1, 3);

            strTemp = strTemp.Replace('h', ' ').Trim(' ');

            // Determines if the handle belong to a No Plug&Play hardware 
            //
            m_PcanHandle = Convert.ToUInt16(strTemp, 16);
            bNonPnP = m_PcanHandle <= PCANBasic.PCAN_DNGBUS1;
            // Activates/deactivates configuration controls according with the 
            // kind of hardware
            //
            //*  cbbHwType.Enabled = bNonPnP;
            //*  cbbIO.Enabled = bNonPnP;
            //*cbbInterrupt.Enabled = bNonPnP;
        }

        public TPCANStatus Init(TPCANBaudrate baudrate)//;object sender, EventArgs e)
        {
            try
            {
                btnHwRefresh_Click(this, new EventArgs());
            }
            catch
            {
                MessageBox.Show("CAN Error 1");
            }

            try
            {
                cbbChannel_SelectedIndexChanged(this, new EventArgs());
            }
            catch
            {
                MessageBox.Show("CAN Error 2");
            }

            TPCANStatus stsResult = TPCANStatus.PCAN_ERROR_CAUTION;
            try
            {
                

                m_Baudrate = baudrate;
                m_HwType = TPCANType.PCAN_TYPE_ISA;

                stsResult = PCANBasic.Initialize(
                    m_PcanHandle,
                    m_Baudrate,
                    m_HwType,
                    Convert.ToUInt32("0x100", 16),//cbbIO.Text, 16),
                    Convert.ToUInt16("3"));// cbbInterrupt.Text));
            }
            catch
            {
                MessageBox.Show("CAN Error 3");
            }

            //btnHwRefresh_Click(this, new EventArgs());
           

            

            // Connects a selected PCAN-Basic channel
            //
            /*
            if (m_IsFD)
                stsResult = PCANBasic.InitializeFD(
                    m_PcanHandle,
                    txtBitrate.Text);
            else
                */


            



           


            return stsResult;
            /*
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                if (stsResult != TPCANStatus.PCAN_ERROR_CAUTION)
                    MessageBox.Show(GetFormatedError(stsResult));
                else
                {
                    IncludeTextMessage("******************************************************");
                    IncludeTextMessage("The bitrate being used is different than the given one");
                    IncludeTextMessage("******************************************************");
                    stsResult = TPCANStatus.PCAN_ERROR_OK;
                }
            else
                // Prepares the PCAN-Basic's PCAN-Trace file
                //
                ConfigureTraceFile();

            // Sets the connection status of the main-form
            //
            */
            //*SetConnectionStatus(stsResult == TPCANStatus.PCAN_ERROR_OK);
        }

        public void Dispose()
        {

           // PCANBasic.Uninitialize();
        }


        public void InitializeBasicComponents()
        {
            // Creates the list for received messages
            //
            m_LastMsgsList = new System.Collections.ArrayList();
            // Creates the delegate used for message reading
            //
            m_ReadDelegate = new ReadDelegateHandler(ReadMessages);
            // Creates the event used for signalize incomming messages 
            //
            m_ReceiveEvent = new System.Threading.AutoResetEvent(false);
            // Creates an array with all possible PCAN-Channels
            //
            m_HandlesArray = new TPCANHandle[]
            {
                PCANBasic.PCAN_ISABUS1,
                PCANBasic.PCAN_ISABUS2,
                PCANBasic.PCAN_ISABUS3,
                PCANBasic.PCAN_ISABUS4,
                PCANBasic.PCAN_ISABUS5,
                PCANBasic.PCAN_ISABUS6,
                PCANBasic.PCAN_ISABUS7,
                PCANBasic.PCAN_ISABUS8,
                PCANBasic.PCAN_DNGBUS1,
                PCANBasic.PCAN_PCIBUS1,
                PCANBasic.PCAN_PCIBUS2,
                PCANBasic.PCAN_PCIBUS3,
                PCANBasic.PCAN_PCIBUS4,
                PCANBasic.PCAN_PCIBUS5,
                PCANBasic.PCAN_PCIBUS6,
                PCANBasic.PCAN_PCIBUS7,
                PCANBasic.PCAN_PCIBUS8,
                PCANBasic.PCAN_PCIBUS9,
                PCANBasic.PCAN_PCIBUS10,
                PCANBasic.PCAN_PCIBUS11,
                PCANBasic.PCAN_PCIBUS12,
                PCANBasic.PCAN_PCIBUS13,
                PCANBasic.PCAN_PCIBUS14,
                PCANBasic.PCAN_PCIBUS15,
                PCANBasic.PCAN_PCIBUS16,
                PCANBasic.PCAN_USBBUS1,
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8,
                PCANBasic.PCAN_USBBUS9,
                PCANBasic.PCAN_USBBUS10,
                PCANBasic.PCAN_USBBUS11,
                PCANBasic.PCAN_USBBUS12,
                PCANBasic.PCAN_USBBUS13,
                PCANBasic.PCAN_USBBUS14,
                PCANBasic.PCAN_USBBUS15,
                PCANBasic.PCAN_USBBUS16,
                PCANBasic.PCAN_PCCBUS1,
                PCANBasic.PCAN_PCCBUS2,
                PCANBasic.PCAN_LANBUS1,
                PCANBasic.PCAN_LANBUS2,
                PCANBasic.PCAN_LANBUS3,
                PCANBasic.PCAN_LANBUS4,
                PCANBasic.PCAN_LANBUS5,
                PCANBasic.PCAN_LANBUS6,
                PCANBasic.PCAN_LANBUS7,
                PCANBasic.PCAN_LANBUS8,
                PCANBasic.PCAN_LANBUS9,
                PCANBasic.PCAN_LANBUS10,
                PCANBasic.PCAN_LANBUS11,
                PCANBasic.PCAN_LANBUS12,
                PCANBasic.PCAN_LANBUS13,
                PCANBasic.PCAN_LANBUS14,
                PCANBasic.PCAN_LANBUS15,
                PCANBasic.PCAN_LANBUS16,
            };

            // Fills and configures the Data of several comboBox components
            //
       //*     FillComboBoxData();

            // Prepares the PCAN-Basic's debug-Log file
            //
       //*     ConfigureLogFile();
        }

        private void InsertMsgEntry(TPCANMsgFD newMsg, TPCANTimestampFD timeStamp)
        {
        }

            private void ProcessMessage(TPCANMsgFD theMsg, TPCANTimestampFD itsTimeStamp)
        {
            // We search if a message (Same ID and Type) is 
            // already received or if this is a new message
            //
            lock (m_LastMsgsList.SyncRoot)
            {
                foreach (MessageStatus msg in m_LastMsgsList)
                {
                    if ((msg.CANMsg.ID == theMsg.ID) && (msg.CANMsg.MSGTYPE == theMsg.MSGTYPE))
                    {
                        // Modify the message and exit
                        //
                        msg.Update(theMsg, itsTimeStamp);
                        return;
                    }
                }
                // Message not found. It will created
                //
                InsertMsgEntry(theMsg, itsTimeStamp);
            }
        }



        public TPCANStatus WriteFrame(string name, uint id, uint dlc, byte[] bytes)
        {
            if (bytes.Length < dlc)
                return TPCANStatus.PCAN_ERROR_UNKNOWN;
            TPCANMsg CANMsg;
       //     TextBox txtbCurrentTextBox;

            // We create a TPCANMsg message structure 
            //
            CANMsg = new TPCANMsg();
            CANMsg.DATA = new byte[8];

            // We configurate the Message.  The ID,
            // Length of the Data, Message Type
            // and the data
            //

            if (id >= 0x800)
                CANMsg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;

            CANMsg.ID = id;// Convert.ToUInt32(txtID.Text, 16);
            CANMsg.LEN = (byte)dlc;// bytes.Length;// Convert.ToByte(nudLength.Value);
                                            //  CANMsg.MSGTYPE = (chbExtended.Checked) ? TPCANMessageType.PCAN_MESSAGE_EXTENDED : TPCANMessageType.PCAN_MESSAGE_STANDARD;
                                            // If a remote frame will be sent, the data bytes are not important.
                                            //
            for (int i = 0; i < dlc; i++)
            {
                CANMsg.DATA[i] = bytes[i];
            }
            /*
            if (chbRemote.Checked)
                CANMsg.MSGTYPE |= TPCANMessageType.PCAN_MESSAGE_RTR;
            else
            {
                // We get so much data as the Len of the message
                //
                for (int i = 0; i < GetLengthFromDLC(CANMsg.LEN, true); i++)
                {
                    txtbCurrentTextBox = (TextBox)this.Controls.Find("txtData" + i.ToString(), true)[0];
                    CANMsg.DATA[i] = Convert.ToByte(txtbCurrentTextBox.Text, 16);
                }
            }
            */
            // The message is sent to the configured hardware
            //
            return PCANBasic.Write(m_PcanHandle, ref CANMsg);
        }

        /// <summary>
        /// Processes a received message, in order to show it in the Message-ListView
        /// </summary>
        /// <param name="theMsg">The received PCAN-Basic message</param>
        /// <returns>True if the message must be created, false if it must be modified</returns>
        private void ProcessMessage(TPCANMsg theMsg, TPCANTimestamp itsTimeStamp)
        {
            TPCANMsgFD newMsg;
            TPCANTimestampFD newTimestamp;

            newMsg = new TPCANMsgFD();
            newMsg.DATA = new byte[64];
            newMsg.ID = theMsg.ID;
            newMsg.DLC = theMsg.LEN;
            for (int i = 0; i < ((theMsg.LEN > 8) ? 8 : theMsg.LEN); i++)
                newMsg.DATA[i] = theMsg.DATA[i];
            newMsg.MSGTYPE = theMsg.MSGTYPE;

            newTimestamp = Convert.ToUInt64(itsTimeStamp.micros + 1000 * itsTimeStamp.millis + 0x100000000 * 1000 * itsTimeStamp.millis_overflow);
            ProcessMessage(newMsg, newTimestamp);
        }


        private TPCANStatus ReadMessage()
        {
            TPCANMsg CANMsg;
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.Read(m_PcanHandle, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                // We process the received message
                //
                ProcessMessage(CANMsg, CANTimeStamp);

            return stsResult;
        }

        private TPCANStatus ReadMessageFD()
        {
            TPCANMsgFD CANMsg;
            TPCANTimestampFD CANTimeStamp;
            TPCANStatus stsResult;

            // We execute the "Read" function of the PCANBasic                
            //
            stsResult = PCANBasic.ReadFD(m_PcanHandle, out CANMsg, out CANTimeStamp);
            if (stsResult != TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                // We process the received message
                //
                ProcessMessage(CANMsg, CANTimeStamp);

            return stsResult;
        }

        public bool Enabled = true;

        /// <summary>
        /// Function for reading PCAN-Basic messages
        /// </summary>
        public void ReadMessages()
        {
            TPCANStatus stsResult;

            // We read at least one time the queue looking for messages.
            // If a message is found, we look again trying to find more.
            // If the queue is empty or an error occurr, we get out from
            // the dowhile statement.
            //			
            do
            {
                stsResult = m_IsFD ? ReadMessageFD() : ReadMessage();
                if (stsResult == TPCANStatus.PCAN_ERROR_ILLOPERATION)
                    break;
            } while (Enabled && (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
        }

        /// <summary>
        /// Saves the desired connection mode
        /// </summary>
        private bool m_IsFD;
        /// <summary>
        /// Saves the handle of a PCAN hardware
        /// </summary>
        private TPCANHandle m_PcanHandle;
        /// <summary>
        /// Saves the baudrate register for a conenction
        /// </summary>
        private TPCANBaudrate m_Baudrate;
        /// <summary>
        /// Saves the type of a non-plug-and-play hardware
        /// </summary>
        private TPCANType m_HwType;
        /// <summary>
        /// Stores the status of received messages for its display
        /// </summary>
        private System.Collections.ArrayList m_LastMsgsList;
        /// <summary>
        /// Read Delegate for calling the function "ReadMessages"
        /// </summary>
        private ReadDelegateHandler m_ReadDelegate;
        /// <summary>
        /// Receive-Event
        /// </summary>
        private System.Threading.AutoResetEvent m_ReceiveEvent;
        /// <summary>
        /// Thread for message reading (using events)
        /// </summary>
        private System.Threading.Thread m_ReadThread;
        /// <summary>
        /// Handles of the current available PCAN-Hardware
        /// </summary>
        private TPCANHandle[] m_HandlesArray;

    }
}
