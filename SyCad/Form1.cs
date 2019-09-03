using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SyCad
{
    public partial class Form1 : Form
    {
        Capture _capture;
        bool _streaming;
        public Form1()
        {
            InitializeComponent();
        }

        private void txtPesquisar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if(string.IsNullOrEmpty(txtPesquisar.Text) || txtPesquisar.Text == "")
                {
                    this.visitantesTableAdapter.Fill(this.appData.Visitantes);
                    visitantesBindingSource.DataSource = this.appData.Visitantes;
                    //visitantesBindingSource.DataSource = this.appData.Visitantes;
                }
                else
                {
                    var query = from o in this.appData.Visitantes
                                where o.Nome.Contains(txtPesquisar.Text) || o.Telefone == txtPesquisar.Text || o.RG == txtPesquisar.Text
                                select o;
                    visitantesBindingSource.DataSource = query.ToList();
                    //dataGridView.DataSource = query.ToList();
                }
            }
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Você tem certeza que deseja deletar esse registro?", "Deletar Registro", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) 
                {
                    try
                    {
                        visitantesBindingSource.RemoveCurrent();
                        visitantesBindingSource.EndEdit();
                        visitantesTableAdapter.Update(this.appData.Visitantes);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPEG|*.jpg", ValidateNames = true, Multiselect = false }) 
                {
                    if(ofd.ShowDialog()==DialogResult.OK)
                    {
                        pictureBox.Image = Image.FromFile(ofd.FileName);
                    }
                }
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            try
            {
                panel.Enabled = true;
                txtNome.Focus();
                this.appData.Visitantes.AddVisitantesRow(this.appData.Visitantes.NewVisitantesRow());
                visitantesBindingSource.MoveLast();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                visitantesBindingSource.ResetBindings(false);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            panel.Enabled = true;
            txtNome.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            panel.Enabled = false;
            visitantesBindingSource.ResetBindings(false);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                visitantesBindingSource.EndEdit();
                visitantesTableAdapter.Update(this.appData.Visitantes);
                panel.Enabled = false;
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                visitantesBindingSource.ResetBindings(false);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.visitantesTableAdapter.Fill(this.appData.Visitantes);
            visitantesBindingSource.DataSource = this.appData.Visitantes;
            _streaming = false;
            _capture = new Capture();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var img = _capture.QueryFrame().ToImage<Bgr, byte>();
            var bmp = img.Bitmap;
            pictureBox.Image = bmp;
            Application.Idle -= streaming;
        }

        private void streaming(object sender, System.EventArgs e)
        {
            var img = _capture.QueryFrame().ToImage<Bgr, byte>();
            var bmp = img.Bitmap;
            pictureBox.Image = bmp;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!_streaming)
            {
                Application.Idle += streaming;
            }
            else
            {
               
            }
            _streaming = !_streaming;
        }

        private void txtTelefone_Leave(object sender, EventArgs e)
        {
            txtTelefone.Text = AplicarMascaraTelefone(txtTelefone.Text);
        }

        string AplicarMascaraTelefone(string strNumero)
        {
            string strMascara = "{0:(00)0000-0000}";
            strNumero = strNumero.Replace("(", "").Replace(")", "").Replace("-","");

            if (string.IsNullOrEmpty(strNumero))
            {
                return "";
            }
            else
            {
                long lngNumero = Convert.ToInt64((String)strNumero);

                if (strNumero.Length == 11)
                {
                    strMascara = "{0:(00)00000-0000}";
                }

                else if (strNumero.Length == 9)
                {
                    strMascara = "{0:00000-0000}";
                }
                else if (strNumero.Length == 8)
                {
                    strMascara = "{0:0000-0000}";
                }

                return string.Format(strMascara, lngNumero);
            }
        }

        string AplicarMascaraID(string strNumero)
        {
            //string strMascara = "{0:000.000.000-00}";
            //if (string.IsNullOrEmpty(strNumero) || strNumero == "0")
            //{
            //    return "0";
            //}
            //else
            //{
            //    if (strNumero.Length == 11)
            //    {
            //        strMascara = "{0:000.000.000-00}";
            //        long lngNumero = Convert.ToInt64(strNumero);
            //        return string.Format(strMascara, lngNumero);
            //    }
            //    else
            //    {
            //        strMascara = "{0:000.000.000-00}";
            //        long lngNumero = Convert.ToInt64(strNumero);
            //        return string.Format(strMascara, lngNumero);
            //    }
            //}
            strNumero = strNumero.Replace(".", "").Replace("-", "");
            return Convert.ToUInt64((String)strNumero).ToString(@"000\.000\.000\-00");
        }

        private void txtTelefone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtRG_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtRG_Leave(object sender, EventArgs e)
        {
            String mensagem = "";
            String texto = txtRG.Text;
            if (ValidaCPF.IsCpf(texto))
            {
                mensagem = "O número é um CPF Válido !";
                txtRG.Text = AplicarMascaraID(txtRG.Text);
            }
            else
            {
                mensagem = "O número é um CPF Inválido !";
                txtRG.Clear();
            }
            MessageBox.Show(mensagem, "Validação");


        }

    }
}
