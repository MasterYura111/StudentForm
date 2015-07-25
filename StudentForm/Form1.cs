﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Newtonsoft.Json;
using StudentForm.Models;


namespace StudentForm
{
    public partial class Form1 : Form
    {
        private Uri baseUri = new Uri("http://localhost:19121/");

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void LoadStudents()
        {
            List<Student> students = null;
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("http://localhost:19121/api/values").Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    var jsonString = response.Content.ReadAsStringAsync();
                    students = JsonConvert.DeserializeObject<List<Student>>(jsonString.Result);
                    dataGridView1.DataSource = students;
                }
            }
        }



        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            student.Name = textBoxName.Text;
            student.Surname = textBoxSurname.Text;
            student.Age = (int)numericUpDownAge.Value;

            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                var response = client.PostAsync("api/values",

                    new StringContent(JsonConvert.SerializeObject(student).ToString(),
                        Encoding.UTF8, "application/json"))
                        .Result;
            }
            LoadStudents();
            ClearGroupAdd();
            ShowTable();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            int id = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            string Name = (string)dataGridView1.Rows[e.RowIndex].Cells[1].Value;
            string Surname = (string)dataGridView1.Rows[e.RowIndex].Cells[2].Value;

            if (e.ColumnIndex == 4) //edit
            {

                groupBoxAdd.Visible = false;
                groupBoxEdit.Visible = true;
                dataGridView1.Visible = false;
                buttonShowAddGroup.Visible = false;

                var client = new HttpClient();

                var response = client.GetAsync(baseUri + "/api/values/" + id).Result;

                Student student = null;
                var jsonString = response.Content.ReadAsStringAsync();
                student = JsonConvert.DeserializeObject<Student>(jsonString.Result);

                labelEditId.Text = student.Id.ToString();
                textBoxEditName.Text = student.Name;
                textBoxEditSurname.Text = student.Surname;
                numericUpDownEditAge.Value = student.Age;

            }
            else if (e.ColumnIndex == 5) //delete
            {

                DialogResult result = MessageBox.Show(
                    "Delete: " + Name + " " + Surname + " ?",
                    "Submit",
                    MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = baseUri;
                    var response = client.DeleteAsync("api/values/" + id)
                            .Result;
                }
                LoadStudents();

            }

            return;
        }

        private void buttonEditSave_Click(object sender, EventArgs e)
        {
            Student student = new Student();
            student.Id = Int32.Parse(labelEditId.Text);
            student.Name = textBoxEditName.Text;
            student.Age = (int)numericUpDownEditAge.Value;
            student.Surname = textBoxEditSurname.Text;

            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                var response = client.PutAsync("api/values/" + student.Id,

                    new StringContent(JsonConvert.SerializeObject(student).ToString(),
                        Encoding.UTF8, "application/json"))
                        .Result;
            }
            LoadStudents();

        }

        private void buttonShowAddGroup_Click(object sender, EventArgs e)
        {
            buttonShowAddGroup.Visible = false;
            groupBoxAdd.Visible = true;
            groupBoxEdit.Visible = false;
            dataGridView1.Visible = false;

        }


        private void ShowTable()
        {
            buttonShowAddGroup.Visible = true;
            groupBoxAdd.Visible = false;
            groupBoxEdit.Visible = false;
            dataGridView1.Visible = true;
        }

        private void ClearGroupAdd()
        {
            textBoxName.Clear();
            textBoxSurname.Clear();
            numericUpDownAge.Value = 0;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTable();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowTable();
        }


    }

}