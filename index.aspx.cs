using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TXTextControl;
using TXTextControl.DocumentServer.Fields;

namespace tx_checkboxes
{
    public partial class index : System.Web.UI.Page
    {

        // Helper method to return the string representation
        // of a unicode character
        private static string UnicodeHexToString(string text)
        {
            return System.Text.Encoding.Unicode.GetString(BitConverter.GetBytes(short.Parse(text.Substring(2), System.Globalization.NumberStyles.HexNumber)));
        }

        // constants for the checked and unchecked characters
        private string UNCHECKED = UnicodeHexToString("\\u2610");
        private string CHECKED = UnicodeHexToString("\\u2612");

        protected void Page_Load(object sender, EventArgs e)
        {
            // handle the AJAX postback
            string eventTarget = Convert.ToString(Request.Params.Get("__EVENTTARGET"));
            string eventArgument = Convert.ToString(Request.Params.Get("__EVENTARGUMENT"));

            // if the event argument is set, toggle the checkbox
            if (eventArgument != null)
            {
                ToggleCheckBox(eventArgument);
            }

            // load a document at the first start
            if (!IsPostBack)
            {
                byte[] data;

                using (TXTextControl.ServerTextControl tx = new ServerTextControl())
                {
                    tx.Create();
                    TXTextControl.LoadSettings ls = new LoadSettings();
                    ls.ApplicationFieldFormat = ApplicationFieldFormat.MSWord;
                    tx.Load(Server.MapPath("template.docx"), StreamType.WordprocessingML, ls);
                    tx.Save(out data, BinaryStreamType.InternalUnicodeFormat);

                    data = ProcessCheckboxFields(data);
                }

                TextControl1.LoadTextAsync(data, TXTextControl.Web.BinaryStreamType.InternalUnicodeFormat);
            }
        }

        /********************************************************
         * ToggleCheckBox method
         * desc:        toggles a specific checkbox field
         * parameter:   fieldname - the name of the field
        ********************************************************/
        private void ToggleCheckBox(string fieldName)
        {
            byte[] data;

            // create a new temporary ServerTextControl
            using (TXTextControl.ServerTextControl tx = new ServerTextControl())
            {
                tx.Create();

                // save current document from the editor and load
                // it into the ServerTextControl
                TextControl1.SaveText(out data, TXTextControl.Web.BinaryStreamType.InternalUnicodeFormat);
                tx.Load(data, BinaryStreamType.InternalUnicodeFormat);

                // loop through all ApplicationFields in each TextPart
                foreach (IFormattedText textPart in tx.TextParts)
                {
                    foreach (ApplicationField field in textPart.ApplicationFields)
                    {
                        if ((field.TypeName != "FORMCHECKBOX"))
                            continue;

                        // if the field is a checkbox and the name matches
                        // toggle the Checked property
                        if (field.Name == fieldName)
                        {
                            // create a new adapter field
                            FormCheckBox checkboxField = new FormCheckBox(field);
                            checkboxField.Checked = !checkboxField.Checked;
                        }
                    }
                }

                tx.Save(out data, BinaryStreamType.InternalUnicodeFormat);
            }

            // process the fields and load the document back into the editor
            TextControl1.LoadTextAsync(ProcessCheckboxFields(data), TXTextControl.Web.BinaryStreamType.InternalUnicodeFormat);
        }

        /********************************************************
         * ProcessCheckboxFields method
         * desc:        Sets the Unicode characters for all
         *              checkbox fields
         * parameter:   document - the document in the internal
         *              Text Control format
        ********************************************************/
        private byte[] ProcessCheckboxFields(byte[] document)
        {
            // create a new temporary ServerTextControl
            using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl())
            {
                // load the document
                tx.Create();
                tx.Load(document, TXTextControl.BinaryStreamType.InternalUnicodeFormat);

                // loop through all ApplicationFields
                foreach (IFormattedText textPart in tx.TextParts)
                {
                    foreach (ApplicationField field in textPart.ApplicationFields)
                    {
                        if ((field.TypeName != "FORMCHECKBOX"))
                            return null;

                        // create a new adapter field
                        FormCheckBox checkboxField = new FormCheckBox(field);

                        // select the field to change the font name
                        textPart.Selection.Start = checkboxField.Start - 1;
                        textPart.Selection.Length = checkboxField.Length;

                        textPart.Selection.FontName = "Arial Unicode MS";

                        checkboxField.Text = checkboxField.Checked == true ? CHECKED : UNCHECKED;
                    }
                }

                tx.Save(out document, BinaryStreamType.InternalUnicodeFormat);
                return document;
            }
        }
    }
}