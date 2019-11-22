﻿using SupportDataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SupportTools
{
    public partial class Form1 : Form
    {



        public DatabaseManager ConectDB()
        {

            DatabaseManager BancoConectado = new DatabaseManager("Data Source=localhost\\SQLEXPRESS;Integrated Security=True;Connect Timeout=999;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=PersonalMed;");

            return BancoConectado;
        }
               

        public Form1()
        {   
            MessageBox.Show("ANTES DE REALIZAR A EXCLUSÃO FAÇA UM BKP!");
            InitializeComponent();
            AtualizarCombo();
            
        }



        private void AtualizarCombo()
        {


            var us01Items = ConectDB().GetDatable("select * from us01 where usertype = 'M'");

            comboBox1.DataSource = us01Items;
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "UserNumber";

            var ItensDuplicados = ConectDB().GetDatable($@"select * FROM AGE03 WHERE AGE03_ID IN (
                                    select AGE03.AGE03_ID from age03 inner join
                                    (select data,inicio,usernumber, COUNT(*) AS C from AGE03
                                    where ATIVO = 'T' and ATIVO = 'F'
                                    group by DATA,INICIO,USERNUMBER
                                    having COUNT (*) > 1) duplicatas 

                                    on age03.DATA = duplicatas.DATA 
                                    AND AGE03.INICIO = duplicatas.INICIO 
                                    AND AGE03.USERNUMBER = duplicatas.USERNUMBER
                                    WHERE
                                    AGE03.ATIVO='T' and AGE03.ATIVO='F')");

            comboBox2.DataSource = ItensDuplicados;
            comboBox2.DisplayMember = "Data";

            var CpfPacienteVazio = ConectDB().GetDatable($@"select * from Clini_01 
                                    WHERE CLINI_01_cpf IN (    
        
                                    select CLINI_01.Clini_01_CPF from CLINI_01 inner join

                                    (select clini_01_cpf, COUNT(*) AS C from CLINI_01
                                    where ATIVO = 'T' and Clini_01_CPF = ''
                                    group by clini_01_cpf
                                    having COUNT (*) > 1) duplicatas 
                                    on CLINI_01.Clini_01_CPF = duplicatas.Clini_01_CPF 
                                    WHERE
                                    CLINI_01.ATIVO='T')");
            comboBox3.DataSource = CpfPacienteVazio;
            comboBox3.DisplayMember = "Name";

            var CpfPaciente = ConectDB().GetDatable($@"select * from Clini_01 
                                    WHERE CLINI_01_cpf IN (    
        
                                    select CLINI_01.Clini_01_CPF from CLINI_01 inner join

                                    (select clini_01_cpf, COUNT(*) AS C from CLINI_01
                                    where ATIVO = 'T' and Clini_01_CPF != ''
                                    group by clini_01_cpf
                                    having COUNT (*) > 1) duplicatas 
                                    on CLINI_01.Clini_01_CPF = duplicatas.Clini_01_CPF 
                                    WHERE
                                    CLINI_01.ATIVO='T')");
            comboBox4.DataSource = CpfPaciente;
            comboBox4.DisplayMember = "Name";

            var CpfInvalido = ConectDB().GetDatable($@"select replace (REPLACE(REPLACE(clini_01_cpf,'.',''),'-',''), ' ', '') ,clini_01_cpf 
                                    from CLINI_01 
                                    where clini_01_cpf is not null");

            comboBox5.DataSource = CpfInvalido;
            comboBox5.DisplayMember = "clini_01_cpf";

            var CpfMaiorQueDoze = ConectDB().GetDatable($@"SELECT Clini_01_CPF,*
                                    FROM clini_01
                                    WHERE LEN (Clini_01_CPF) > 11");

            comboBox6.DataSource = CpfMaiorQueDoze;
            comboBox6.DisplayMember = "Clini_01_CPF";


        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"update DAT005 set FIELD = 'PCOD' where TABLENAME='clini_01'
                    
                DECLARE @TABLENAME VARCHAR(255);
                DECLARE @FIELDNAME VARCHAR(255);
                DECLARE @NEXTKEY INT;
                DECLARE @SQLString NVARCHAR(255);
                DECLARE @ParmDefinition NVARCHAR(255);

                DECLARE CUR_TEMP CURSOR FOR
                SELECT  TABLENAME, FIELD
                FROM DAT005 d
                INNER JOIN DBO.SYSOBJECTS  s
                   ON (s.ID = OBJECT_ID(d.TABLENAME)) AND (OBJECTPROPERTY(s.ID, N'ISUSERTABLE') = 1)
                WHERE TABLENAME IS NOT NULL
                  AND FIELD IS NOT NULL
                ORDER BY TABLENAME

                OPEN CUR_TEMP
                FETCH NEXT FROM CUR_TEMP INTO @TABLENAME, @FIELDNAME
                WHILE (@@FETCH_STATUS = 0)
                BEGIN
                      SET @SQLString = N'select @NEXTKEY_OUT = COALESCE(max('+ (@FIELDNAME) +') + 1, 1) from [' + @TABLENAME +']';
                      SET @ParmDefinition = N'@NEXTKEY_OUT int OUTPUT';
                      EXECUTE sp_executesql @SQLString, @ParmDefinition, @NEXTKEY_OUT = @NEXTKEY OUTPUT;
           
                      UPDATE DAT005
                         SET NEXTKEY = @NEXTKEY
                         WHERE TABLENAME = @TABLENAME

                      FETCH NEXT FROM CUR_TEMP INTO @TABLENAME, @FIELDNAME
                END
                CLOSE CUR_TEMP
                DEALLOCATE CUR_TEMP

            
                ");
                MessageBox.Show("Concluido");

            }
            catch (Exception)
            {
                MessageBox.Show("Script Concluido");
            }



            



        }

        private void button2_Click(object sender, EventArgs e)
        {
            //DatabaseManager db = new DatabaseManager("Data Source=localhost\\SQLEXPRESS;Integrated Security=True;Connect Timeout=999;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=PersonalMed;");
            try
            {
                ConectDB().ExecuteNonQueries($@"UPDATE FIN_02 SET CLOUD_SYNC_DATE = '2014-05-06 17:35:28.453', CLOUD_SYNC_ID = 2 WHERE TABLENAME = 'PROPTB_TUSS'

                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.757' WHERE PROPTB_TUSS_ID = 1
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.760' WHERE PROPTB_TUSS_ID = 2
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 3
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 4
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 6 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 5
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 7 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 6
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 8 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 7
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 9 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 8
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 10 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 9
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 11 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.763' WHERE PROPTB_TUSS_ID = 10
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 12 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 11
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 13 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 12
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 14 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 13
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 15 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 14
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 16 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 15
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 17 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 16
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 18 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 17
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 19 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.767' WHERE PROPTB_TUSS_ID = 18
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 20 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 19
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 21 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 20
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 22 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 21
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 23 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 22
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 24 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 23
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 25 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 24
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 26 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.770' WHERE PROPTB_TUSS_ID = 25
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 27 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.773' WHERE PROPTB_TUSS_ID = 26
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 28 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.773' WHERE PROPTB_TUSS_ID = 27
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 29 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.773' WHERE PROPTB_TUSS_ID = 28
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 30 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.773' WHERE PROPTB_TUSS_ID = 29
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 31 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.773' WHERE PROPTB_TUSS_ID = 30
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 32 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 31
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 33 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 32
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 34 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 33
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 35 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 34
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 36 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 35
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 37 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.777' WHERE PROPTB_TUSS_ID = 36
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 38 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.780' WHERE PROPTB_TUSS_ID = 37
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 39 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.780' WHERE PROPTB_TUSS_ID = 38
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 40 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.780' WHERE PROPTB_TUSS_ID = 39
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 41 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.780' WHERE PROPTB_TUSS_ID = 40
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 42 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.783' WHERE PROPTB_TUSS_ID = 41
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 43 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.783' WHERE PROPTB_TUSS_ID = 42
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 44 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.783' WHERE PROPTB_TUSS_ID = 43
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 45 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.787' WHERE PROPTB_TUSS_ID = 44
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 46 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.787' WHERE PROPTB_TUSS_ID = 45
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 47 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.787' WHERE PROPTB_TUSS_ID = 46
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 48 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.787' WHERE PROPTB_TUSS_ID = 47
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 49 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.790' WHERE PROPTB_TUSS_ID = 48
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 50 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.790' WHERE PROPTB_TUSS_ID = 49
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 51 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.790' WHERE PROPTB_TUSS_ID = 50
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 52 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.793' WHERE PROPTB_TUSS_ID = 51
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 53 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.793' WHERE PROPTB_TUSS_ID = 52
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 54 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.797' WHERE PROPTB_TUSS_ID = 53
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 55 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.797' WHERE PROPTB_TUSS_ID = 54
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 56 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.797' WHERE PROPTB_TUSS_ID = 55
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 57 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.800' WHERE PROPTB_TUSS_ID = 56
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 58 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.803' WHERE PROPTB_TUSS_ID = 57
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 59 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.803' WHERE PROPTB_TUSS_ID = 58
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 60 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.803' WHERE PROPTB_TUSS_ID = 59
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 61 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.807' WHERE PROPTB_TUSS_ID = 60
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 62 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.807' WHERE PROPTB_TUSS_ID = 61
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 63 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.807' WHERE PROPTB_TUSS_ID = 62
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 64 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.810' WHERE PROPTB_TUSS_ID = 63
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 65 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.810' WHERE PROPTB_TUSS_ID = 64
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 66 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.813' WHERE PROPTB_TUSS_ID = 65
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 67 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.813' WHERE PROPTB_TUSS_ID = 66
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 68 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.817' WHERE PROPTB_TUSS_ID = 67
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 69 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.817' WHERE PROPTB_TUSS_ID = 68
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 70 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.817' WHERE PROPTB_TUSS_ID = 69
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 71 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.820' WHERE PROPTB_TUSS_ID = 70
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 72 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.820' WHERE PROPTB_TUSS_ID = 71
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 73 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.823' WHERE PROPTB_TUSS_ID = 72
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 74 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.823' WHERE PROPTB_TUSS_ID = 73
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 75 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.827' WHERE PROPTB_TUSS_ID = 74
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 76 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.827' WHERE PROPTB_TUSS_ID = 75
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 77 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.827' WHERE PROPTB_TUSS_ID = 76
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 78 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.830' WHERE PROPTB_TUSS_ID = 77
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 79 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.837' WHERE PROPTB_TUSS_ID = 78
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 80 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.837' WHERE PROPTB_TUSS_ID = 79
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 81 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.837' WHERE PROPTB_TUSS_ID = 80
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 82 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.840' WHERE PROPTB_TUSS_ID = 81
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 83 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.840' WHERE PROPTB_TUSS_ID = 82
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 84 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.843' WHERE PROPTB_TUSS_ID = 83
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 85 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.843' WHERE PROPTB_TUSS_ID = 84
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 86 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.847' WHERE PROPTB_TUSS_ID = 85
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 87 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.847' WHERE PROPTB_TUSS_ID = 86
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 88 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.850' WHERE PROPTB_TUSS_ID = 87
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 89 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.850' WHERE PROPTB_TUSS_ID = 88
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 90 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.853' WHERE PROPTB_TUSS_ID = 89
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 91 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.857' WHERE PROPTB_TUSS_ID = 90
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 92 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.860' WHERE PROPTB_TUSS_ID = 91
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 93 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.863' WHERE PROPTB_TUSS_ID = 92
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 94 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.867' WHERE PROPTB_TUSS_ID = 93
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 95 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.867' WHERE PROPTB_TUSS_ID = 94
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 96 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.870' WHERE PROPTB_TUSS_ID = 95
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 97 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.870' WHERE PROPTB_TUSS_ID = 96
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 98 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.873' WHERE PROPTB_TUSS_ID = 97
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 99 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.877' WHERE PROPTB_TUSS_ID = 98
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 100 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.880' WHERE PROPTB_TUSS_ID = 99
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 101 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.883' WHERE PROPTB_TUSS_ID = 100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 102 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.887' WHERE PROPTB_TUSS_ID = 101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 103 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.890' WHERE PROPTB_TUSS_ID = 102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 104 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.890' WHERE PROPTB_TUSS_ID = 103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 105 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.897' WHERE PROPTB_TUSS_ID = 104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 106 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.900' WHERE PROPTB_TUSS_ID = 105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 107 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.903' WHERE PROPTB_TUSS_ID = 106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 108 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.903' WHERE PROPTB_TUSS_ID = 107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 109 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.907' WHERE PROPTB_TUSS_ID = 108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 110 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.907' WHERE PROPTB_TUSS_ID = 109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 111 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.910' WHERE PROPTB_TUSS_ID = 110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 112 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.913' WHERE PROPTB_TUSS_ID = 111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 113 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.920' WHERE PROPTB_TUSS_ID = 112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 114 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.923' WHERE PROPTB_TUSS_ID = 113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 115 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.923' WHERE PROPTB_TUSS_ID = 114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 116 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.927' WHERE PROPTB_TUSS_ID = 115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 117 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.927' WHERE PROPTB_TUSS_ID = 116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 118 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.930' WHERE PROPTB_TUSS_ID = 117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 119 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.933' WHERE PROPTB_TUSS_ID = 118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 120 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.937' WHERE PROPTB_TUSS_ID = 119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 121 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.940' WHERE PROPTB_TUSS_ID = 120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 122 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.943' WHERE PROPTB_TUSS_ID = 121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 123 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.943' WHERE PROPTB_TUSS_ID = 122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 124 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.947' WHERE PROPTB_TUSS_ID = 123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 125 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.950' WHERE PROPTB_TUSS_ID = 124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 126 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.950' WHERE PROPTB_TUSS_ID = 125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 127 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.953' WHERE PROPTB_TUSS_ID = 126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 128 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.957' WHERE PROPTB_TUSS_ID = 127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 129 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.960' WHERE PROPTB_TUSS_ID = 128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 130 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.963' WHERE PROPTB_TUSS_ID = 129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 131 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.967' WHERE PROPTB_TUSS_ID = 130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 132 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.970' WHERE PROPTB_TUSS_ID = 131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 133 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.970' WHERE PROPTB_TUSS_ID = 132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 134 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.973' WHERE PROPTB_TUSS_ID = 133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 135 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.977' WHERE PROPTB_TUSS_ID = 134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 136 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.980' WHERE PROPTB_TUSS_ID = 135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 137 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.983' WHERE PROPTB_TUSS_ID = 136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 138 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.987' WHERE PROPTB_TUSS_ID = 137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 139 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.990' WHERE PROPTB_TUSS_ID = 138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 140 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.990' WHERE PROPTB_TUSS_ID = 139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 141 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.993' WHERE PROPTB_TUSS_ID = 140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 142 , CLOUD_SYNC_DATE = '2014-05-06 17:35:28.997' WHERE PROPTB_TUSS_ID = 141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 143 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.000' WHERE PROPTB_TUSS_ID = 142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 144 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.003' WHERE PROPTB_TUSS_ID = 143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 145 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.007' WHERE PROPTB_TUSS_ID = 144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 146 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.010' WHERE PROPTB_TUSS_ID = 145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 147 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.010' WHERE PROPTB_TUSS_ID = 146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 148 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.013' WHERE PROPTB_TUSS_ID = 147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 149 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.020' WHERE PROPTB_TUSS_ID = 148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 150 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.020' WHERE PROPTB_TUSS_ID = 149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 151 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.023' WHERE PROPTB_TUSS_ID = 150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 152 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.027' WHERE PROPTB_TUSS_ID = 151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 153 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.030' WHERE PROPTB_TUSS_ID = 152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 154 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.033' WHERE PROPTB_TUSS_ID = 153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 155 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.037' WHERE PROPTB_TUSS_ID = 154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 156 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.040' WHERE PROPTB_TUSS_ID = 155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 157 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.043' WHERE PROPTB_TUSS_ID = 156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 158 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.047' WHERE PROPTB_TUSS_ID = 157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 159 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.047' WHERE PROPTB_TUSS_ID = 158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 160 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.053' WHERE PROPTB_TUSS_ID = 159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 161 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.057' WHERE PROPTB_TUSS_ID = 160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 162 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.060' WHERE PROPTB_TUSS_ID = 161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 163 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.063' WHERE PROPTB_TUSS_ID = 162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 164 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.067' WHERE PROPTB_TUSS_ID = 163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 165 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.067' WHERE PROPTB_TUSS_ID = 164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 166 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.073' WHERE PROPTB_TUSS_ID = 165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 167 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.077' WHERE PROPTB_TUSS_ID = 166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 168 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.080' WHERE PROPTB_TUSS_ID = 167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 169 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.083' WHERE PROPTB_TUSS_ID = 168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 170 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.087' WHERE PROPTB_TUSS_ID = 169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 171 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.087' WHERE PROPTB_TUSS_ID = 170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 172 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.093' WHERE PROPTB_TUSS_ID = 171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 173 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.097' WHERE PROPTB_TUSS_ID = 172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 174 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.100' WHERE PROPTB_TUSS_ID = 173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 175 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.103' WHERE PROPTB_TUSS_ID = 174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 176 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.107' WHERE PROPTB_TUSS_ID = 175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 177 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.110' WHERE PROPTB_TUSS_ID = 176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 178 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.113' WHERE PROPTB_TUSS_ID = 177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 179 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.117' WHERE PROPTB_TUSS_ID = 178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 180 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.120' WHERE PROPTB_TUSS_ID = 179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 181 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.127' WHERE PROPTB_TUSS_ID = 180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 182 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.130' WHERE PROPTB_TUSS_ID = 181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 183 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.137' WHERE PROPTB_TUSS_ID = 182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 184 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.140' WHERE PROPTB_TUSS_ID = 183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 185 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.143' WHERE PROPTB_TUSS_ID = 184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 186 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.147' WHERE PROPTB_TUSS_ID = 185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 187 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.150' WHERE PROPTB_TUSS_ID = 186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 188 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.157' WHERE PROPTB_TUSS_ID = 187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 189 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.157' WHERE PROPTB_TUSS_ID = 188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 190 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.163' WHERE PROPTB_TUSS_ID = 189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 191 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.167' WHERE PROPTB_TUSS_ID = 190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 192 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.170' WHERE PROPTB_TUSS_ID = 191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 193 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.173' WHERE PROPTB_TUSS_ID = 192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 194 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.177' WHERE PROPTB_TUSS_ID = 193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 195 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.180' WHERE PROPTB_TUSS_ID = 194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 196 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.187' WHERE PROPTB_TUSS_ID = 195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 197 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.190' WHERE PROPTB_TUSS_ID = 196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 198 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.193' WHERE PROPTB_TUSS_ID = 197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 199 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.200' WHERE PROPTB_TUSS_ID = 198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 200 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.203' WHERE PROPTB_TUSS_ID = 199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 201 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.210' WHERE PROPTB_TUSS_ID = 200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 202 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.213' WHERE PROPTB_TUSS_ID = 201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 203 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.217' WHERE PROPTB_TUSS_ID = 202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 204 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.223' WHERE PROPTB_TUSS_ID = 203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 205 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.227' WHERE PROPTB_TUSS_ID = 204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 206 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.230' WHERE PROPTB_TUSS_ID = 205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 207 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.233' WHERE PROPTB_TUSS_ID = 206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 208 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.240' WHERE PROPTB_TUSS_ID = 207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 209 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.243' WHERE PROPTB_TUSS_ID = 208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 210 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.247' WHERE PROPTB_TUSS_ID = 209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 211 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.250' WHERE PROPTB_TUSS_ID = 210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 212 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.253' WHERE PROPTB_TUSS_ID = 211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 213 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.260' WHERE PROPTB_TUSS_ID = 212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 214 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.263' WHERE PROPTB_TUSS_ID = 213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 215 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.267' WHERE PROPTB_TUSS_ID = 214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 216 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.270' WHERE PROPTB_TUSS_ID = 215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 217 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.277' WHERE PROPTB_TUSS_ID = 216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 218 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.280' WHERE PROPTB_TUSS_ID = 217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 219 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.287' WHERE PROPTB_TUSS_ID = 218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 220 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.290' WHERE PROPTB_TUSS_ID = 219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 221 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.300' WHERE PROPTB_TUSS_ID = 220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 222 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.303' WHERE PROPTB_TUSS_ID = 221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 223 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.307' WHERE PROPTB_TUSS_ID = 222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 224 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.313' WHERE PROPTB_TUSS_ID = 223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 225 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.317' WHERE PROPTB_TUSS_ID = 224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 226 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.323' WHERE PROPTB_TUSS_ID = 225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 227 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.327' WHERE PROPTB_TUSS_ID = 226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 228 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.330' WHERE PROPTB_TUSS_ID = 227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 229 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.337' WHERE PROPTB_TUSS_ID = 228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 230 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.343' WHERE PROPTB_TUSS_ID = 229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 231 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.347' WHERE PROPTB_TUSS_ID = 230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 232 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.350' WHERE PROPTB_TUSS_ID = 231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 233 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.357' WHERE PROPTB_TUSS_ID = 232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 234 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.363' WHERE PROPTB_TUSS_ID = 233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 235 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.367' WHERE PROPTB_TUSS_ID = 234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 236 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.373' WHERE PROPTB_TUSS_ID = 235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 237 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.377' WHERE PROPTB_TUSS_ID = 236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 238 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.383' WHERE PROPTB_TUSS_ID = 237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 239 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.387' WHERE PROPTB_TUSS_ID = 238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 240 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.393' WHERE PROPTB_TUSS_ID = 239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 241 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.397' WHERE PROPTB_TUSS_ID = 240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 242 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.403' WHERE PROPTB_TUSS_ID = 241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 243 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.407' WHERE PROPTB_TUSS_ID = 242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 244 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.410' WHERE PROPTB_TUSS_ID = 243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 245 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.417' WHERE PROPTB_TUSS_ID = 244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 246 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.423' WHERE PROPTB_TUSS_ID = 245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 247 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.427' WHERE PROPTB_TUSS_ID = 246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 248 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.430' WHERE PROPTB_TUSS_ID = 247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 249 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.437' WHERE PROPTB_TUSS_ID = 248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 250 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.443' WHERE PROPTB_TUSS_ID = 249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 251 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.447' WHERE PROPTB_TUSS_ID = 250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 252 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.453' WHERE PROPTB_TUSS_ID = 251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 253 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.460' WHERE PROPTB_TUSS_ID = 252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 254 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.467' WHERE PROPTB_TUSS_ID = 253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 255 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.473' WHERE PROPTB_TUSS_ID = 254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 256 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.480' WHERE PROPTB_TUSS_ID = 255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 257 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.483' WHERE PROPTB_TUSS_ID = 256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 258 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.490' WHERE PROPTB_TUSS_ID = 257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 259 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.493' WHERE PROPTB_TUSS_ID = 258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 260 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.500' WHERE PROPTB_TUSS_ID = 259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 261 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.507' WHERE PROPTB_TUSS_ID = 260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 262 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.510' WHERE PROPTB_TUSS_ID = 261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 263 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.517' WHERE PROPTB_TUSS_ID = 262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 264 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.523' WHERE PROPTB_TUSS_ID = 263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 265 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.527' WHERE PROPTB_TUSS_ID = 264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 266 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.533' WHERE PROPTB_TUSS_ID = 265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 267 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.540' WHERE PROPTB_TUSS_ID = 266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 268 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.547' WHERE PROPTB_TUSS_ID = 267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 269 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.553' WHERE PROPTB_TUSS_ID = 268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 270 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.557' WHERE PROPTB_TUSS_ID = 269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 271 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.563' WHERE PROPTB_TUSS_ID = 270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 272 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.570' WHERE PROPTB_TUSS_ID = 271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 273 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.577' WHERE PROPTB_TUSS_ID = 272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 274 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.583' WHERE PROPTB_TUSS_ID = 273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 275 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.587' WHERE PROPTB_TUSS_ID = 274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 276 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.593' WHERE PROPTB_TUSS_ID = 275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 277 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.597' WHERE PROPTB_TUSS_ID = 276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 278 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.607' WHERE PROPTB_TUSS_ID = 277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 279 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.613' WHERE PROPTB_TUSS_ID = 278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 280 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.617' WHERE PROPTB_TUSS_ID = 279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 281 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.627' WHERE PROPTB_TUSS_ID = 280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 282 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.630' WHERE PROPTB_TUSS_ID = 281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 283 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.637' WHERE PROPTB_TUSS_ID = 282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 284 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.643' WHERE PROPTB_TUSS_ID = 283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 285 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.650' WHERE PROPTB_TUSS_ID = 284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 286 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.657' WHERE PROPTB_TUSS_ID = 285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 287 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.667' WHERE PROPTB_TUSS_ID = 286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 288 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.670' WHERE PROPTB_TUSS_ID = 287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 289 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.677' WHERE PROPTB_TUSS_ID = 288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 290 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.680' WHERE PROPTB_TUSS_ID = 289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 291 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.687' WHERE PROPTB_TUSS_ID = 290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 292 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.693' WHERE PROPTB_TUSS_ID = 291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 293 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.700' WHERE PROPTB_TUSS_ID = 292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 294 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.707' WHERE PROPTB_TUSS_ID = 293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 295 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.713' WHERE PROPTB_TUSS_ID = 294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 296 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.717' WHERE PROPTB_TUSS_ID = 295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 297 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.727' WHERE PROPTB_TUSS_ID = 296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 298 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.730' WHERE PROPTB_TUSS_ID = 297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 299 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.737' WHERE PROPTB_TUSS_ID = 298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 300 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.747' WHERE PROPTB_TUSS_ID = 299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 301 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.757' WHERE PROPTB_TUSS_ID = 300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 302 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.767' WHERE PROPTB_TUSS_ID = 301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 303 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.777' WHERE PROPTB_TUSS_ID = 302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 304 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.783' WHERE PROPTB_TUSS_ID = 303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 305 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.790' WHERE PROPTB_TUSS_ID = 304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 306 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.800' WHERE PROPTB_TUSS_ID = 305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 307 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.807' WHERE PROPTB_TUSS_ID = 306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 308 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.813' WHERE PROPTB_TUSS_ID = 307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 309 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.823' WHERE PROPTB_TUSS_ID = 308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 310 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.830' WHERE PROPTB_TUSS_ID = 309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 311 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.837' WHERE PROPTB_TUSS_ID = 310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 312 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.843' WHERE PROPTB_TUSS_ID = 311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 313 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.850' WHERE PROPTB_TUSS_ID = 312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 314 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.857' WHERE PROPTB_TUSS_ID = 313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 315 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.870' WHERE PROPTB_TUSS_ID = 314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 316 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.877' WHERE PROPTB_TUSS_ID = 315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 317 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.880' WHERE PROPTB_TUSS_ID = 316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 318 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.890' WHERE PROPTB_TUSS_ID = 317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 319 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.897' WHERE PROPTB_TUSS_ID = 318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 320 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.903' WHERE PROPTB_TUSS_ID = 319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 321 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.917' WHERE PROPTB_TUSS_ID = 320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 322 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.923' WHERE PROPTB_TUSS_ID = 321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 323 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.930' WHERE PROPTB_TUSS_ID = 322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 324 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.943' WHERE PROPTB_TUSS_ID = 323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 325 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.950' WHERE PROPTB_TUSS_ID = 324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 326 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.957' WHERE PROPTB_TUSS_ID = 325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 327 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.973' WHERE PROPTB_TUSS_ID = 326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 328 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.987' WHERE PROPTB_TUSS_ID = 327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 329 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.993' WHERE PROPTB_TUSS_ID = 328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 330 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.000' WHERE PROPTB_TUSS_ID = 329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 331 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.007' WHERE PROPTB_TUSS_ID = 330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 332 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.017' WHERE PROPTB_TUSS_ID = 331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 333 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.023' WHERE PROPTB_TUSS_ID = 332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 334 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.030' WHERE PROPTB_TUSS_ID = 333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 335 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.040' WHERE PROPTB_TUSS_ID = 334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 336 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.047' WHERE PROPTB_TUSS_ID = 335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 337 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.057' WHERE PROPTB_TUSS_ID = 336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 338 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.073' WHERE PROPTB_TUSS_ID = 337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 339 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.080' WHERE PROPTB_TUSS_ID = 338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 340 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.087' WHERE PROPTB_TUSS_ID = 339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 341 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.100' WHERE PROPTB_TUSS_ID = 340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 342 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.113' WHERE PROPTB_TUSS_ID = 341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 343 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.120' WHERE PROPTB_TUSS_ID = 342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 344 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.130' WHERE PROPTB_TUSS_ID = 343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 345 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.137' WHERE PROPTB_TUSS_ID = 344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 346 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.147' WHERE PROPTB_TUSS_ID = 345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 347 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.153' WHERE PROPTB_TUSS_ID = 346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 348 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.160' WHERE PROPTB_TUSS_ID = 347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 349 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.167' WHERE PROPTB_TUSS_ID = 348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 350 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.177' WHERE PROPTB_TUSS_ID = 349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 351 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.183' WHERE PROPTB_TUSS_ID = 350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 352 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.190' WHERE PROPTB_TUSS_ID = 351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 353 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.197' WHERE PROPTB_TUSS_ID = 352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 354 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.207' WHERE PROPTB_TUSS_ID = 353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 355 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.213' WHERE PROPTB_TUSS_ID = 354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 356 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.220' WHERE PROPTB_TUSS_ID = 355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 357 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.233' WHERE PROPTB_TUSS_ID = 356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 358 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.243' WHERE PROPTB_TUSS_ID = 357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 359 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.253' WHERE PROPTB_TUSS_ID = 358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 360 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.260' WHERE PROPTB_TUSS_ID = 359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 361 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.267' WHERE PROPTB_TUSS_ID = 360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 362 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.277' WHERE PROPTB_TUSS_ID = 361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 363 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.283' WHERE PROPTB_TUSS_ID = 362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 364 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.290' WHERE PROPTB_TUSS_ID = 363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 365 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.300' WHERE PROPTB_TUSS_ID = 364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 366 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.307' WHERE PROPTB_TUSS_ID = 365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 367 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.317' WHERE PROPTB_TUSS_ID = 366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 368 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.327' WHERE PROPTB_TUSS_ID = 367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 369 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.333' WHERE PROPTB_TUSS_ID = 368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 370 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.347' WHERE PROPTB_TUSS_ID = 369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 371 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.353' WHERE PROPTB_TUSS_ID = 370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 372 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.363' WHERE PROPTB_TUSS_ID = 371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 373 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.370' WHERE PROPTB_TUSS_ID = 372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 374 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.377' WHERE PROPTB_TUSS_ID = 373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 375 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.397' WHERE PROPTB_TUSS_ID = 374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 376 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.407' WHERE PROPTB_TUSS_ID = 375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 377 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.417' WHERE PROPTB_TUSS_ID = 376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 378 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.423' WHERE PROPTB_TUSS_ID = 377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 379 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.430' WHERE PROPTB_TUSS_ID = 378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 380 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.437' WHERE PROPTB_TUSS_ID = 379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 381 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.443' WHERE PROPTB_TUSS_ID = 380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 382 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.453' WHERE PROPTB_TUSS_ID = 381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 383 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.460' WHERE PROPTB_TUSS_ID = 382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 384 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.470' WHERE PROPTB_TUSS_ID = 383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 385 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.477' WHERE PROPTB_TUSS_ID = 384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 386 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.483' WHERE PROPTB_TUSS_ID = 385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 387 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.493' WHERE PROPTB_TUSS_ID = 386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 388 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.503' WHERE PROPTB_TUSS_ID = 387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 389 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.513' WHERE PROPTB_TUSS_ID = 388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 390 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.520' WHERE PROPTB_TUSS_ID = 389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 391 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.527' WHERE PROPTB_TUSS_ID = 390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 392 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.540' WHERE PROPTB_TUSS_ID = 391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 393 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.547' WHERE PROPTB_TUSS_ID = 392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 394 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.557' WHERE PROPTB_TUSS_ID = 393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 395 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.563' WHERE PROPTB_TUSS_ID = 394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 396 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.573' WHERE PROPTB_TUSS_ID = 395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 397 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.580' WHERE PROPTB_TUSS_ID = 396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 398 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.590' WHERE PROPTB_TUSS_ID = 397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 399 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.600' WHERE PROPTB_TUSS_ID = 398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 400 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.610' WHERE PROPTB_TUSS_ID = 399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 401 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.620' WHERE PROPTB_TUSS_ID = 400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 402 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.627' WHERE PROPTB_TUSS_ID = 401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 403 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.637' WHERE PROPTB_TUSS_ID = 402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 404 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.643' WHERE PROPTB_TUSS_ID = 403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 405 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.653' WHERE PROPTB_TUSS_ID = 404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 406 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.667' WHERE PROPTB_TUSS_ID = 405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 407 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.673' WHERE PROPTB_TUSS_ID = 406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 408 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.683' WHERE PROPTB_TUSS_ID = 407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 409 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.690' WHERE PROPTB_TUSS_ID = 408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 410 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.700' WHERE PROPTB_TUSS_ID = 409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 411 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.707' WHERE PROPTB_TUSS_ID = 410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 412 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.717' WHERE PROPTB_TUSS_ID = 411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 413 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.727' WHERE PROPTB_TUSS_ID = 412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 414 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.733' WHERE PROPTB_TUSS_ID = 413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 415 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.743' WHERE PROPTB_TUSS_ID = 414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 416 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.750' WHERE PROPTB_TUSS_ID = 415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 417 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.763' WHERE PROPTB_TUSS_ID = 416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 418 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.770' WHERE PROPTB_TUSS_ID = 417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 419 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.780' WHERE PROPTB_TUSS_ID = 418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 420 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.790' WHERE PROPTB_TUSS_ID = 419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 421 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.797' WHERE PROPTB_TUSS_ID = 420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 422 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.807' WHERE PROPTB_TUSS_ID = 421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 423 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.817' WHERE PROPTB_TUSS_ID = 422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 424 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.827' WHERE PROPTB_TUSS_ID = 423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 425 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.833' WHERE PROPTB_TUSS_ID = 424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 426 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.847' WHERE PROPTB_TUSS_ID = 425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 427 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.857' WHERE PROPTB_TUSS_ID = 426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 428 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.867' WHERE PROPTB_TUSS_ID = 427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 429 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.873' WHERE PROPTB_TUSS_ID = 428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 430 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.883' WHERE PROPTB_TUSS_ID = 429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 431 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.893' WHERE PROPTB_TUSS_ID = 430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 432 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.903' WHERE PROPTB_TUSS_ID = 431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 433 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.910' WHERE PROPTB_TUSS_ID = 432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 434 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.920' WHERE PROPTB_TUSS_ID = 433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 435 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.930' WHERE PROPTB_TUSS_ID = 434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 436 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.940' WHERE PROPTB_TUSS_ID = 435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 437 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.950' WHERE PROPTB_TUSS_ID = 436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 438 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.960' WHERE PROPTB_TUSS_ID = 437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 439 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.967' WHERE PROPTB_TUSS_ID = 438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 440 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.977' WHERE PROPTB_TUSS_ID = 439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 441 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.987' WHERE PROPTB_TUSS_ID = 440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 442 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.000' WHERE PROPTB_TUSS_ID = 441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 443 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.007' WHERE PROPTB_TUSS_ID = 442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 444 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.020' WHERE PROPTB_TUSS_ID = 443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 445 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.027' WHERE PROPTB_TUSS_ID = 444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 446 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.037' WHERE PROPTB_TUSS_ID = 445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 447 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.047' WHERE PROPTB_TUSS_ID = 446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 448 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.057' WHERE PROPTB_TUSS_ID = 447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 449 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.067' WHERE PROPTB_TUSS_ID = 448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 450 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.077' WHERE PROPTB_TUSS_ID = 449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 451 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.087' WHERE PROPTB_TUSS_ID = 450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 452 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.103' WHERE PROPTB_TUSS_ID = 451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 453 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.113' WHERE PROPTB_TUSS_ID = 452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 454 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.123' WHERE PROPTB_TUSS_ID = 453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 455 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.130' WHERE PROPTB_TUSS_ID = 454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 456 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.143' WHERE PROPTB_TUSS_ID = 455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 457 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.150' WHERE PROPTB_TUSS_ID = 456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 458 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.163' WHERE PROPTB_TUSS_ID = 457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 459 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.170' WHERE PROPTB_TUSS_ID = 458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 460 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.183' WHERE PROPTB_TUSS_ID = 459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 461 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.193' WHERE PROPTB_TUSS_ID = 460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 462 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.207' WHERE PROPTB_TUSS_ID = 461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 463 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.217' WHERE PROPTB_TUSS_ID = 462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 464 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.227' WHERE PROPTB_TUSS_ID = 463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 465 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.237' WHERE PROPTB_TUSS_ID = 464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 466 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.250' WHERE PROPTB_TUSS_ID = 465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 467 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.263' WHERE PROPTB_TUSS_ID = 466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 468 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.277' WHERE PROPTB_TUSS_ID = 467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 469 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.283' WHERE PROPTB_TUSS_ID = 468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 470 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.297' WHERE PROPTB_TUSS_ID = 469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 471 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.307' WHERE PROPTB_TUSS_ID = 470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 472 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.317' WHERE PROPTB_TUSS_ID = 471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 473 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.323' WHERE PROPTB_TUSS_ID = 472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 474 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.337' WHERE PROPTB_TUSS_ID = 473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 475 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.343' WHERE PROPTB_TUSS_ID = 474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 476 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.357' WHERE PROPTB_TUSS_ID = 475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 477 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.363' WHERE PROPTB_TUSS_ID = 476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 478 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.377' WHERE PROPTB_TUSS_ID = 477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 479 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.383' WHERE PROPTB_TUSS_ID = 478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 480 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.397' WHERE PROPTB_TUSS_ID = 479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 481 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.403' WHERE PROPTB_TUSS_ID = 480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 482 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.417' WHERE PROPTB_TUSS_ID = 481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 483 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.423' WHERE PROPTB_TUSS_ID = 482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 484 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.433' WHERE PROPTB_TUSS_ID = 483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 485 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.443' WHERE PROPTB_TUSS_ID = 484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 486 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.453' WHERE PROPTB_TUSS_ID = 485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 487 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.463' WHERE PROPTB_TUSS_ID = 486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 488 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.473' WHERE PROPTB_TUSS_ID = 487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 489 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.483' WHERE PROPTB_TUSS_ID = 488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 490 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.493' WHERE PROPTB_TUSS_ID = 489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 491 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.503' WHERE PROPTB_TUSS_ID = 490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 492 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.513' WHERE PROPTB_TUSS_ID = 491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 493 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.523' WHERE PROPTB_TUSS_ID = 492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 494 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.533' WHERE PROPTB_TUSS_ID = 493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 495 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.547' WHERE PROPTB_TUSS_ID = 494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 496 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.557' WHERE PROPTB_TUSS_ID = 495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 497 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.567' WHERE PROPTB_TUSS_ID = 496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 498 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.577' WHERE PROPTB_TUSS_ID = 497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 499 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.587' WHERE PROPTB_TUSS_ID = 498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 500 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.597' WHERE PROPTB_TUSS_ID = 499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 501 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.607' WHERE PROPTB_TUSS_ID = 500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 502 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.617' WHERE PROPTB_TUSS_ID = 501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 503 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.630' WHERE PROPTB_TUSS_ID = 502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 504 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.637' WHERE PROPTB_TUSS_ID = 503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 505 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.650' WHERE PROPTB_TUSS_ID = 504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 506 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.663' WHERE PROPTB_TUSS_ID = 505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 507 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.673' WHERE PROPTB_TUSS_ID = 506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 508 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.687' WHERE PROPTB_TUSS_ID = 507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 509 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.693' WHERE PROPTB_TUSS_ID = 508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 510 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.707' WHERE PROPTB_TUSS_ID = 509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 511 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.717' WHERE PROPTB_TUSS_ID = 510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 512 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.727' WHERE PROPTB_TUSS_ID = 511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 513 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.737' WHERE PROPTB_TUSS_ID = 512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 514 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.750' WHERE PROPTB_TUSS_ID = 513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 515 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.760' WHERE PROPTB_TUSS_ID = 514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 516 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.773' WHERE PROPTB_TUSS_ID = 515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 517 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.787' WHERE PROPTB_TUSS_ID = 516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 518 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.797' WHERE PROPTB_TUSS_ID = 517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 519 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.807' WHERE PROPTB_TUSS_ID = 518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 520 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.820' WHERE PROPTB_TUSS_ID = 519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 521 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.833' WHERE PROPTB_TUSS_ID = 520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 522 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.843' WHERE PROPTB_TUSS_ID = 521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 523 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.853' WHERE PROPTB_TUSS_ID = 522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 524 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.867' WHERE PROPTB_TUSS_ID = 523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 525 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.877' WHERE PROPTB_TUSS_ID = 524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 526 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.887' WHERE PROPTB_TUSS_ID = 525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 527 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.897' WHERE PROPTB_TUSS_ID = 526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 528 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.907' WHERE PROPTB_TUSS_ID = 527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 529 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.917' WHERE PROPTB_TUSS_ID = 528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 530 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.930' WHERE PROPTB_TUSS_ID = 529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 531 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.940' WHERE PROPTB_TUSS_ID = 530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 532 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.950' WHERE PROPTB_TUSS_ID = 531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 533 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.963' WHERE PROPTB_TUSS_ID = 532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 534 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.973' WHERE PROPTB_TUSS_ID = 533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 535 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.983' WHERE PROPTB_TUSS_ID = 534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 536 , CLOUD_SYNC_DATE = '2014-05-06 17:35:31.993' WHERE PROPTB_TUSS_ID = 535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 537 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.007' WHERE PROPTB_TUSS_ID = 536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 538 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.017' WHERE PROPTB_TUSS_ID = 537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 539 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.027' WHERE PROPTB_TUSS_ID = 538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 540 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.040' WHERE PROPTB_TUSS_ID = 539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 541 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.047' WHERE PROPTB_TUSS_ID = 540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 542 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.060' WHERE PROPTB_TUSS_ID = 541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 543 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.073' WHERE PROPTB_TUSS_ID = 542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 544 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.083' WHERE PROPTB_TUSS_ID = 543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 545 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.093' WHERE PROPTB_TUSS_ID = 544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 546 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.103' WHERE PROPTB_TUSS_ID = 545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 547 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.117' WHERE PROPTB_TUSS_ID = 546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 548 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.127' WHERE PROPTB_TUSS_ID = 547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 549 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.140' WHERE PROPTB_TUSS_ID = 548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 550 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.150' WHERE PROPTB_TUSS_ID = 549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 551 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.163' WHERE PROPTB_TUSS_ID = 550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 552 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.177' WHERE PROPTB_TUSS_ID = 551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 553 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.187' WHERE PROPTB_TUSS_ID = 552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 554 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.200' WHERE PROPTB_TUSS_ID = 553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 555 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.210' WHERE PROPTB_TUSS_ID = 554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 556 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.220' WHERE PROPTB_TUSS_ID = 555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 557 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.233' WHERE PROPTB_TUSS_ID = 556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 558 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.243' WHERE PROPTB_TUSS_ID = 557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 559 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.257' WHERE PROPTB_TUSS_ID = 558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 560 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.267' WHERE PROPTB_TUSS_ID = 559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 561 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.277' WHERE PROPTB_TUSS_ID = 560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 562 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.290' WHERE PROPTB_TUSS_ID = 561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 563 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.300' WHERE PROPTB_TUSS_ID = 562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 564 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.313' WHERE PROPTB_TUSS_ID = 563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 565 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.327' WHERE PROPTB_TUSS_ID = 564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 566 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.337' WHERE PROPTB_TUSS_ID = 565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 567 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.347' WHERE PROPTB_TUSS_ID = 566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 568 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.357' WHERE PROPTB_TUSS_ID = 567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 569 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.370' WHERE PROPTB_TUSS_ID = 568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 570 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.383' WHERE PROPTB_TUSS_ID = 569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 571 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.393' WHERE PROPTB_TUSS_ID = 570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 572 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.407' WHERE PROPTB_TUSS_ID = 571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 573 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.417' WHERE PROPTB_TUSS_ID = 572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 574 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.430' WHERE PROPTB_TUSS_ID = 573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 575 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.440' WHERE PROPTB_TUSS_ID = 574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 576 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.450' WHERE PROPTB_TUSS_ID = 575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 577 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.463' WHERE PROPTB_TUSS_ID = 576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 578 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.477' WHERE PROPTB_TUSS_ID = 577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 579 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.487' WHERE PROPTB_TUSS_ID = 578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 580 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.500' WHERE PROPTB_TUSS_ID = 579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 581 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.510' WHERE PROPTB_TUSS_ID = 580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 582 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.523' WHERE PROPTB_TUSS_ID = 581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 583 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.537' WHERE PROPTB_TUSS_ID = 582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 584 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.547' WHERE PROPTB_TUSS_ID = 583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 585 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.560' WHERE PROPTB_TUSS_ID = 584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 586 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.573' WHERE PROPTB_TUSS_ID = 585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 587 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.587' WHERE PROPTB_TUSS_ID = 586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 588 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.597' WHERE PROPTB_TUSS_ID = 587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 589 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.613' WHERE PROPTB_TUSS_ID = 588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 590 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.623' WHERE PROPTB_TUSS_ID = 589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 591 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.637' WHERE PROPTB_TUSS_ID = 590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 592 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.647' WHERE PROPTB_TUSS_ID = 591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 593 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.667' WHERE PROPTB_TUSS_ID = 592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 594 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.680' WHERE PROPTB_TUSS_ID = 593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 595 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.690' WHERE PROPTB_TUSS_ID = 594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 596 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.703' WHERE PROPTB_TUSS_ID = 595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 597 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.717' WHERE PROPTB_TUSS_ID = 596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 598 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.727' WHERE PROPTB_TUSS_ID = 597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 599 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.740' WHERE PROPTB_TUSS_ID = 598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 600 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.753' WHERE PROPTB_TUSS_ID = 599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 601 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.763' WHERE PROPTB_TUSS_ID = 600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 602 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.777' WHERE PROPTB_TUSS_ID = 601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 603 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.790' WHERE PROPTB_TUSS_ID = 602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 604 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.800' WHERE PROPTB_TUSS_ID = 603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 605 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.813' WHERE PROPTB_TUSS_ID = 604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 606 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.827' WHERE PROPTB_TUSS_ID = 605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 607 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.840' WHERE PROPTB_TUSS_ID = 606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 608 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.853' WHERE PROPTB_TUSS_ID = 607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 609 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.863' WHERE PROPTB_TUSS_ID = 608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 610 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.877' WHERE PROPTB_TUSS_ID = 609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 611 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.893' WHERE PROPTB_TUSS_ID = 610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 612 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.903' WHERE PROPTB_TUSS_ID = 611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 613 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.917' WHERE PROPTB_TUSS_ID = 612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 614 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.930' WHERE PROPTB_TUSS_ID = 613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 615 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.940' WHERE PROPTB_TUSS_ID = 614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 616 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.957' WHERE PROPTB_TUSS_ID = 615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 617 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.967' WHERE PROPTB_TUSS_ID = 616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 618 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.980' WHERE PROPTB_TUSS_ID = 617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 619 , CLOUD_SYNC_DATE = '2014-05-06 17:35:32.997' WHERE PROPTB_TUSS_ID = 618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 620 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.010' WHERE PROPTB_TUSS_ID = 619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 621 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.020' WHERE PROPTB_TUSS_ID = 620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 622 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.037' WHERE PROPTB_TUSS_ID = 621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 623 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.050' WHERE PROPTB_TUSS_ID = 622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 624 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.060' WHERE PROPTB_TUSS_ID = 623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 625 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.077' WHERE PROPTB_TUSS_ID = 624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 626 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.090' WHERE PROPTB_TUSS_ID = 625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 627 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.100' WHERE PROPTB_TUSS_ID = 626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 628 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.113' WHERE PROPTB_TUSS_ID = 627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 629 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.127' WHERE PROPTB_TUSS_ID = 628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 630 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.143' WHERE PROPTB_TUSS_ID = 629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 631 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.157' WHERE PROPTB_TUSS_ID = 630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 632 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.170' WHERE PROPTB_TUSS_ID = 631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 633 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.180' WHERE PROPTB_TUSS_ID = 632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 634 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.197' WHERE PROPTB_TUSS_ID = 633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 635 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.210' WHERE PROPTB_TUSS_ID = 634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 636 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.227' WHERE PROPTB_TUSS_ID = 635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 637 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.240' WHERE PROPTB_TUSS_ID = 636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 638 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.260' WHERE PROPTB_TUSS_ID = 637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 639 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.277' WHERE PROPTB_TUSS_ID = 638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 640 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.287' WHERE PROPTB_TUSS_ID = 639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 641 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.303' WHERE PROPTB_TUSS_ID = 640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 642 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.317' WHERE PROPTB_TUSS_ID = 641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 643 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.327' WHERE PROPTB_TUSS_ID = 642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 644 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.340' WHERE PROPTB_TUSS_ID = 643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 645 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.357' WHERE PROPTB_TUSS_ID = 644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 646 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.370' WHERE PROPTB_TUSS_ID = 645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 647 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.387' WHERE PROPTB_TUSS_ID = 646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 648 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.400' WHERE PROPTB_TUSS_ID = 647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 649 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.417' WHERE PROPTB_TUSS_ID = 648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 650 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.427' WHERE PROPTB_TUSS_ID = 649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 651 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.440' WHERE PROPTB_TUSS_ID = 650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 652 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.457' WHERE PROPTB_TUSS_ID = 651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 653 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.470' WHERE PROPTB_TUSS_ID = 652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 654 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.483' WHERE PROPTB_TUSS_ID = 653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 655 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.500' WHERE PROPTB_TUSS_ID = 654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 656 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.513' WHERE PROPTB_TUSS_ID = 655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 657 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.527' WHERE PROPTB_TUSS_ID = 656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 658 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.540' WHERE PROPTB_TUSS_ID = 657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 659 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.557' WHERE PROPTB_TUSS_ID = 658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 660 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.567' WHERE PROPTB_TUSS_ID = 659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 661 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.580' WHERE PROPTB_TUSS_ID = 660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 662 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.597' WHERE PROPTB_TUSS_ID = 661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 663 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.610' WHERE PROPTB_TUSS_ID = 662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 664 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.623' WHERE PROPTB_TUSS_ID = 663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 665 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.637' WHERE PROPTB_TUSS_ID = 664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 666 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.650' WHERE PROPTB_TUSS_ID = 665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 667 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.673' WHERE PROPTB_TUSS_ID = 666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 668 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.687' WHERE PROPTB_TUSS_ID = 667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 669 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.703' WHERE PROPTB_TUSS_ID = 668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 670 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.717' WHERE PROPTB_TUSS_ID = 669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 671 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.730' WHERE PROPTB_TUSS_ID = 670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 672 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.743' WHERE PROPTB_TUSS_ID = 671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 673 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.760' WHERE PROPTB_TUSS_ID = 672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 674 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.773' WHERE PROPTB_TUSS_ID = 673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 675 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.787' WHERE PROPTB_TUSS_ID = 674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 676 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.803' WHERE PROPTB_TUSS_ID = 675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 677 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.817' WHERE PROPTB_TUSS_ID = 676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 678 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.830' WHERE PROPTB_TUSS_ID = 677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 679 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.843' WHERE PROPTB_TUSS_ID = 678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 680 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.857' WHERE PROPTB_TUSS_ID = 679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 681 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.873' WHERE PROPTB_TUSS_ID = 680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 682 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.883' WHERE PROPTB_TUSS_ID = 681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 683 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.900' WHERE PROPTB_TUSS_ID = 682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 684 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.917' WHERE PROPTB_TUSS_ID = 683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 685 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.927' WHERE PROPTB_TUSS_ID = 684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 686 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.943' WHERE PROPTB_TUSS_ID = 685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 687 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.957' WHERE PROPTB_TUSS_ID = 686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 688 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.973' WHERE PROPTB_TUSS_ID = 687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 689 , CLOUD_SYNC_DATE = '2014-05-06 17:35:33.987' WHERE PROPTB_TUSS_ID = 688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 690 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.000' WHERE PROPTB_TUSS_ID = 689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 691 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.017' WHERE PROPTB_TUSS_ID = 690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 692 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.030' WHERE PROPTB_TUSS_ID = 691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 693 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.047' WHERE PROPTB_TUSS_ID = 692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 694 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.063' WHERE PROPTB_TUSS_ID = 693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 695 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.077' WHERE PROPTB_TUSS_ID = 694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 696 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.093' WHERE PROPTB_TUSS_ID = 695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 697 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.103' WHERE PROPTB_TUSS_ID = 696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 698 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.120' WHERE PROPTB_TUSS_ID = 697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 699 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.133' WHERE PROPTB_TUSS_ID = 698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 700 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.150' WHERE PROPTB_TUSS_ID = 699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 701 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.163' WHERE PROPTB_TUSS_ID = 700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 702 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.177' WHERE PROPTB_TUSS_ID = 701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 703 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.197' WHERE PROPTB_TUSS_ID = 702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 704 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.210' WHERE PROPTB_TUSS_ID = 703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 705 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.223' WHERE PROPTB_TUSS_ID = 704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 706 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.240' WHERE PROPTB_TUSS_ID = 705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 707 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.253' WHERE PROPTB_TUSS_ID = 706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 708 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.270' WHERE PROPTB_TUSS_ID = 707
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 709 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.283' WHERE PROPTB_TUSS_ID = 708
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 710 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.300' WHERE PROPTB_TUSS_ID = 709
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 711 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.313' WHERE PROPTB_TUSS_ID = 710
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 712 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.330' WHERE PROPTB_TUSS_ID = 711
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 713 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.343' WHERE PROPTB_TUSS_ID = 712
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 714 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.363' WHERE PROPTB_TUSS_ID = 713
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 715 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.377' WHERE PROPTB_TUSS_ID = 714
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 716 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.393' WHERE PROPTB_TUSS_ID = 715
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 717 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.410' WHERE PROPTB_TUSS_ID = 716
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 718 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.427' WHERE PROPTB_TUSS_ID = 717
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 719 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.443' WHERE PROPTB_TUSS_ID = 718
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 720 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.457' WHERE PROPTB_TUSS_ID = 719
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 721 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.473' WHERE PROPTB_TUSS_ID = 720
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 722 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.487' WHERE PROPTB_TUSS_ID = 721
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 723 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.500' WHERE PROPTB_TUSS_ID = 722
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 724 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.517' WHERE PROPTB_TUSS_ID = 723
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 725 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.533' WHERE PROPTB_TUSS_ID = 724
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 726 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.547' WHERE PROPTB_TUSS_ID = 725
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 727 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.560' WHERE PROPTB_TUSS_ID = 726
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 728 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.577' WHERE PROPTB_TUSS_ID = 727
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 729 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.597' WHERE PROPTB_TUSS_ID = 728
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 730 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.613' WHERE PROPTB_TUSS_ID = 729
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 731 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.627' WHERE PROPTB_TUSS_ID = 730
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 732 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.640' WHERE PROPTB_TUSS_ID = 731
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 733 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.660' WHERE PROPTB_TUSS_ID = 732
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 734 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.677' WHERE PROPTB_TUSS_ID = 733
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 735 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.693' WHERE PROPTB_TUSS_ID = 734
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 736 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.707' WHERE PROPTB_TUSS_ID = 735
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 737 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.720' WHERE PROPTB_TUSS_ID = 736
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 738 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.737' WHERE PROPTB_TUSS_ID = 737
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 739 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.753' WHERE PROPTB_TUSS_ID = 738
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 740 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.770' WHERE PROPTB_TUSS_ID = 739
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 741 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.783' WHERE PROPTB_TUSS_ID = 740
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 742 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.797' WHERE PROPTB_TUSS_ID = 741
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 743 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.813' WHERE PROPTB_TUSS_ID = 742
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 744 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.830' WHERE PROPTB_TUSS_ID = 743
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 745 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.847' WHERE PROPTB_TUSS_ID = 744
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 746 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.860' WHERE PROPTB_TUSS_ID = 745
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 747 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.877' WHERE PROPTB_TUSS_ID = 746
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 748 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.890' WHERE PROPTB_TUSS_ID = 747
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 749 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.907' WHERE PROPTB_TUSS_ID = 748
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 750 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.923' WHERE PROPTB_TUSS_ID = 749
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 751 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.937' WHERE PROPTB_TUSS_ID = 750
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 752 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.953' WHERE PROPTB_TUSS_ID = 751
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 753 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.967' WHERE PROPTB_TUSS_ID = 752
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 754 , CLOUD_SYNC_DATE = '2014-05-06 17:35:34.983' WHERE PROPTB_TUSS_ID = 753
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 755 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.000' WHERE PROPTB_TUSS_ID = 754
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 756 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.017' WHERE PROPTB_TUSS_ID = 755
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 757 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.033' WHERE PROPTB_TUSS_ID = 756
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 758 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.050' WHERE PROPTB_TUSS_ID = 757
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 759 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.067' WHERE PROPTB_TUSS_ID = 758
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 760 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.083' WHERE PROPTB_TUSS_ID = 759
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 761 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.100' WHERE PROPTB_TUSS_ID = 760
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 762 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.113' WHERE PROPTB_TUSS_ID = 761
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 763 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.130' WHERE PROPTB_TUSS_ID = 762
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 764 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.147' WHERE PROPTB_TUSS_ID = 763
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 765 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.160' WHERE PROPTB_TUSS_ID = 764
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 766 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.177' WHERE PROPTB_TUSS_ID = 765
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 767 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.193' WHERE PROPTB_TUSS_ID = 766
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 768 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.207' WHERE PROPTB_TUSS_ID = 767
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 769 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.223' WHERE PROPTB_TUSS_ID = 768
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 770 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.240' WHERE PROPTB_TUSS_ID = 769
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 771 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.257' WHERE PROPTB_TUSS_ID = 770
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 772 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.273' WHERE PROPTB_TUSS_ID = 771
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 773 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.290' WHERE PROPTB_TUSS_ID = 772
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 774 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.303' WHERE PROPTB_TUSS_ID = 773
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 775 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.320' WHERE PROPTB_TUSS_ID = 774
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 776 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.337' WHERE PROPTB_TUSS_ID = 775
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 777 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.353' WHERE PROPTB_TUSS_ID = 776
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 778 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.373' WHERE PROPTB_TUSS_ID = 777
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 779 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.390' WHERE PROPTB_TUSS_ID = 778
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 780 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.403' WHERE PROPTB_TUSS_ID = 779
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 781 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.420' WHERE PROPTB_TUSS_ID = 780
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 782 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.437' WHERE PROPTB_TUSS_ID = 781
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 783 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.457' WHERE PROPTB_TUSS_ID = 782
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 784 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.473' WHERE PROPTB_TUSS_ID = 783
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 785 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.490' WHERE PROPTB_TUSS_ID = 784
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 786 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.503' WHERE PROPTB_TUSS_ID = 785
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 787 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.520' WHERE PROPTB_TUSS_ID = 786
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 788 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.540' WHERE PROPTB_TUSS_ID = 787
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 789 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.557' WHERE PROPTB_TUSS_ID = 788
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 790 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.577' WHERE PROPTB_TUSS_ID = 789
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 791 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.600' WHERE PROPTB_TUSS_ID = 790
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 792 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.620' WHERE PROPTB_TUSS_ID = 791
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 793 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.640' WHERE PROPTB_TUSS_ID = 792
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 794 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.673' WHERE PROPTB_TUSS_ID = 793
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 795 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.703' WHERE PROPTB_TUSS_ID = 794
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 796 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.730' WHERE PROPTB_TUSS_ID = 795
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 797 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.747' WHERE PROPTB_TUSS_ID = 796
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 798 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.767' WHERE PROPTB_TUSS_ID = 797
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 799 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.787' WHERE PROPTB_TUSS_ID = 798
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 800 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.803' WHERE PROPTB_TUSS_ID = 799
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 801 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.827' WHERE PROPTB_TUSS_ID = 800
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 802 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.847' WHERE PROPTB_TUSS_ID = 801
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 803 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.863' WHERE PROPTB_TUSS_ID = 802
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 804 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.880' WHERE PROPTB_TUSS_ID = 803
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 805 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.900' WHERE PROPTB_TUSS_ID = 804
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 806 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.917' WHERE PROPTB_TUSS_ID = 805
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 807 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.937' WHERE PROPTB_TUSS_ID = 806
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 808 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.963' WHERE PROPTB_TUSS_ID = 807
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 809 , CLOUD_SYNC_DATE = '2014-05-06 17:35:35.983' WHERE PROPTB_TUSS_ID = 808
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 810 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.000' WHERE PROPTB_TUSS_ID = 809
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 811 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.020' WHERE PROPTB_TUSS_ID = 810
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 812 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.037' WHERE PROPTB_TUSS_ID = 811
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 813 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.060' WHERE PROPTB_TUSS_ID = 812
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 814 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.097' WHERE PROPTB_TUSS_ID = 813
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 815 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.120' WHERE PROPTB_TUSS_ID = 814
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 816 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.143' WHERE PROPTB_TUSS_ID = 815
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 817 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.163' WHERE PROPTB_TUSS_ID = 816
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 818 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.180' WHERE PROPTB_TUSS_ID = 817
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 819 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.197' WHERE PROPTB_TUSS_ID = 818
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 820 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.217' WHERE PROPTB_TUSS_ID = 819
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 821 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.233' WHERE PROPTB_TUSS_ID = 820
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 822 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.253' WHERE PROPTB_TUSS_ID = 821
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 823 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.267' WHERE PROPTB_TUSS_ID = 822
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 824 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.287' WHERE PROPTB_TUSS_ID = 823
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 825 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.303' WHERE PROPTB_TUSS_ID = 824
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 826 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.323' WHERE PROPTB_TUSS_ID = 825
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 827 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.340' WHERE PROPTB_TUSS_ID = 826
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 828 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.357' WHERE PROPTB_TUSS_ID = 827
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 829 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.377' WHERE PROPTB_TUSS_ID = 828
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 830 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.397' WHERE PROPTB_TUSS_ID = 829
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 831 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.413' WHERE PROPTB_TUSS_ID = 830
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 832 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.433' WHERE PROPTB_TUSS_ID = 831
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 833 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.447' WHERE PROPTB_TUSS_ID = 832
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 834 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.467' WHERE PROPTB_TUSS_ID = 833
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 835 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.483' WHERE PROPTB_TUSS_ID = 834
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 836 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.500' WHERE PROPTB_TUSS_ID = 835
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 837 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.517' WHERE PROPTB_TUSS_ID = 836
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 838 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.537' WHERE PROPTB_TUSS_ID = 837
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 839 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.557' WHERE PROPTB_TUSS_ID = 838
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 840 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.573' WHERE PROPTB_TUSS_ID = 839
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 841 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.590' WHERE PROPTB_TUSS_ID = 840
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 842 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.610' WHERE PROPTB_TUSS_ID = 841
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 843 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.623' WHERE PROPTB_TUSS_ID = 842
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 844 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.640' WHERE PROPTB_TUSS_ID = 843
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 845 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.660' WHERE PROPTB_TUSS_ID = 844
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 846 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.680' WHERE PROPTB_TUSS_ID = 845
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 847 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.700' WHERE PROPTB_TUSS_ID = 846
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 848 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.717' WHERE PROPTB_TUSS_ID = 847
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 849 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.737' WHERE PROPTB_TUSS_ID = 848
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 850 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.757' WHERE PROPTB_TUSS_ID = 849
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 851 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.773' WHERE PROPTB_TUSS_ID = 850
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 852 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.790' WHERE PROPTB_TUSS_ID = 851
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 853 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.807' WHERE PROPTB_TUSS_ID = 852
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 854 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.827' WHERE PROPTB_TUSS_ID = 853
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 855 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.843' WHERE PROPTB_TUSS_ID = 854
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 856 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.860' WHERE PROPTB_TUSS_ID = 855
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 857 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.877' WHERE PROPTB_TUSS_ID = 856
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 858 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.900' WHERE PROPTB_TUSS_ID = 857
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 859 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.920' WHERE PROPTB_TUSS_ID = 858
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 860 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.940' WHERE PROPTB_TUSS_ID = 859
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 861 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.957' WHERE PROPTB_TUSS_ID = 860
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 862 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.977' WHERE PROPTB_TUSS_ID = 861
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 863 , CLOUD_SYNC_DATE = '2014-05-06 17:35:36.993' WHERE PROPTB_TUSS_ID = 862
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 864 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.010' WHERE PROPTB_TUSS_ID = 863
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 865 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.030' WHERE PROPTB_TUSS_ID = 864
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 866 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.047' WHERE PROPTB_TUSS_ID = 865
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 867 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.070' WHERE PROPTB_TUSS_ID = 866
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 868 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.087' WHERE PROPTB_TUSS_ID = 867
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 869 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.107' WHERE PROPTB_TUSS_ID = 868
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 870 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.120' WHERE PROPTB_TUSS_ID = 869
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 871 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.140' WHERE PROPTB_TUSS_ID = 870
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 872 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.157' WHERE PROPTB_TUSS_ID = 871
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 873 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.177' WHERE PROPTB_TUSS_ID = 872
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 874 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.193' WHERE PROPTB_TUSS_ID = 873
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 875 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.210' WHERE PROPTB_TUSS_ID = 874
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 876 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.230' WHERE PROPTB_TUSS_ID = 875
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 877 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.247' WHERE PROPTB_TUSS_ID = 876
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 878 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.263' WHERE PROPTB_TUSS_ID = 877
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 879 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.283' WHERE PROPTB_TUSS_ID = 878
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 880 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.300' WHERE PROPTB_TUSS_ID = 879
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 881 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.320' WHERE PROPTB_TUSS_ID = 880
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 882 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.337' WHERE PROPTB_TUSS_ID = 881
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 883 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.357' WHERE PROPTB_TUSS_ID = 882
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 884 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.373' WHERE PROPTB_TUSS_ID = 883
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 885 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.393' WHERE PROPTB_TUSS_ID = 884
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 886 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.410' WHERE PROPTB_TUSS_ID = 885
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 887 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.427' WHERE PROPTB_TUSS_ID = 886
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 888 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.447' WHERE PROPTB_TUSS_ID = 887
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 889 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.467' WHERE PROPTB_TUSS_ID = 888
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 890 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.483' WHERE PROPTB_TUSS_ID = 889
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 891 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.503' WHERE PROPTB_TUSS_ID = 890
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 892 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.523' WHERE PROPTB_TUSS_ID = 891
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 893 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.543' WHERE PROPTB_TUSS_ID = 892
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 894 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.560' WHERE PROPTB_TUSS_ID = 893
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 895 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.580' WHERE PROPTB_TUSS_ID = 894
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 896 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.597' WHERE PROPTB_TUSS_ID = 895
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 897 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.617' WHERE PROPTB_TUSS_ID = 896
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 898 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.637' WHERE PROPTB_TUSS_ID = 897
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 899 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.657' WHERE PROPTB_TUSS_ID = 898
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 900 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.677' WHERE PROPTB_TUSS_ID = 899
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 901 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.697' WHERE PROPTB_TUSS_ID = 900
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 902 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.717' WHERE PROPTB_TUSS_ID = 901
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 903 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.737' WHERE PROPTB_TUSS_ID = 902
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 904 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.757' WHERE PROPTB_TUSS_ID = 903
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 905 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.773' WHERE PROPTB_TUSS_ID = 904
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 906 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.793' WHERE PROPTB_TUSS_ID = 905
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 907 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.813' WHERE PROPTB_TUSS_ID = 906
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 908 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.830' WHERE PROPTB_TUSS_ID = 907
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 909 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.850' WHERE PROPTB_TUSS_ID = 908
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 910 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.870' WHERE PROPTB_TUSS_ID = 909
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 911 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.887' WHERE PROPTB_TUSS_ID = 910
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 912 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.907' WHERE PROPTB_TUSS_ID = 911
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 913 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.927' WHERE PROPTB_TUSS_ID = 912
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 914 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.947' WHERE PROPTB_TUSS_ID = 913
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 915 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.967' WHERE PROPTB_TUSS_ID = 914
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 916 , CLOUD_SYNC_DATE = '2014-05-06 17:35:37.983' WHERE PROPTB_TUSS_ID = 915
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 917 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.003' WHERE PROPTB_TUSS_ID = 916
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 918 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.023' WHERE PROPTB_TUSS_ID = 917
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 919 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.043' WHERE PROPTB_TUSS_ID = 918
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 920 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.063' WHERE PROPTB_TUSS_ID = 919
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 921 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.083' WHERE PROPTB_TUSS_ID = 920
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 922 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.100' WHERE PROPTB_TUSS_ID = 921
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 923 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.117' WHERE PROPTB_TUSS_ID = 922
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 924 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.137' WHERE PROPTB_TUSS_ID = 923
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 925 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.160' WHERE PROPTB_TUSS_ID = 924
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 926 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.180' WHERE PROPTB_TUSS_ID = 925
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 927 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.200' WHERE PROPTB_TUSS_ID = 926
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 928 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.217' WHERE PROPTB_TUSS_ID = 927
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 929 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.237' WHERE PROPTB_TUSS_ID = 928
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 930 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.257' WHERE PROPTB_TUSS_ID = 929
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 931 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.277' WHERE PROPTB_TUSS_ID = 930
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 932 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.293' WHERE PROPTB_TUSS_ID = 931
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 933 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.313' WHERE PROPTB_TUSS_ID = 932
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 934 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.333' WHERE PROPTB_TUSS_ID = 933
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 935 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.353' WHERE PROPTB_TUSS_ID = 934
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 936 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.377' WHERE PROPTB_TUSS_ID = 935
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 937 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.397' WHERE PROPTB_TUSS_ID = 936
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 938 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.417' WHERE PROPTB_TUSS_ID = 937
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 939 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.437' WHERE PROPTB_TUSS_ID = 938
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 940 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.453' WHERE PROPTB_TUSS_ID = 939
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 941 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.473' WHERE PROPTB_TUSS_ID = 940
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 942 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.493' WHERE PROPTB_TUSS_ID = 941
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 943 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.513' WHERE PROPTB_TUSS_ID = 942
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 944 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.533' WHERE PROPTB_TUSS_ID = 943
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 945 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.553' WHERE PROPTB_TUSS_ID = 944
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 946 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.573' WHERE PROPTB_TUSS_ID = 945
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 947 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.597' WHERE PROPTB_TUSS_ID = 946
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 948 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.617' WHERE PROPTB_TUSS_ID = 947
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 949 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.633' WHERE PROPTB_TUSS_ID = 948
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 950 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.657' WHERE PROPTB_TUSS_ID = 949
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 951 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.680' WHERE PROPTB_TUSS_ID = 950
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 952 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.700' WHERE PROPTB_TUSS_ID = 951
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 953 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.723' WHERE PROPTB_TUSS_ID = 952
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 954 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.743' WHERE PROPTB_TUSS_ID = 953
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 955 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.763' WHERE PROPTB_TUSS_ID = 954
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 956 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.783' WHERE PROPTB_TUSS_ID = 955
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 957 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.803' WHERE PROPTB_TUSS_ID = 956
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 958 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.823' WHERE PROPTB_TUSS_ID = 957
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 959 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.847' WHERE PROPTB_TUSS_ID = 958
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 960 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.863' WHERE PROPTB_TUSS_ID = 959
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 961 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.883' WHERE PROPTB_TUSS_ID = 960
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 962 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.903' WHERE PROPTB_TUSS_ID = 961
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 963 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.927' WHERE PROPTB_TUSS_ID = 962
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 964 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.950' WHERE PROPTB_TUSS_ID = 963
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 965 , CLOUD_SYNC_DATE = '2014-05-06 17:35:38.973' WHERE PROPTB_TUSS_ID = 964
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 966 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.003' WHERE PROPTB_TUSS_ID = 965
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 967 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.027' WHERE PROPTB_TUSS_ID = 966
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 968 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.047' WHERE PROPTB_TUSS_ID = 967
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 969 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.067' WHERE PROPTB_TUSS_ID = 968
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 970 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.087' WHERE PROPTB_TUSS_ID = 969
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 971 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.113' WHERE PROPTB_TUSS_ID = 970
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 972 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.133' WHERE PROPTB_TUSS_ID = 971
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 973 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.157' WHERE PROPTB_TUSS_ID = 972
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 974 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.177' WHERE PROPTB_TUSS_ID = 973
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 975 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.197' WHERE PROPTB_TUSS_ID = 974
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 976 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.217' WHERE PROPTB_TUSS_ID = 975
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 977 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.240' WHERE PROPTB_TUSS_ID = 976
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 978 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.260' WHERE PROPTB_TUSS_ID = 977
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 979 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.280' WHERE PROPTB_TUSS_ID = 978
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 980 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.300' WHERE PROPTB_TUSS_ID = 979
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 981 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.320' WHERE PROPTB_TUSS_ID = 980
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 982 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.340' WHERE PROPTB_TUSS_ID = 981
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 983 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.363' WHERE PROPTB_TUSS_ID = 982
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 984 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.383' WHERE PROPTB_TUSS_ID = 983
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 985 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.403' WHERE PROPTB_TUSS_ID = 984
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 986 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.423' WHERE PROPTB_TUSS_ID = 985
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 987 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.443' WHERE PROPTB_TUSS_ID = 986
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 988 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.467' WHERE PROPTB_TUSS_ID = 987
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 989 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.487' WHERE PROPTB_TUSS_ID = 988
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 990 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.507' WHERE PROPTB_TUSS_ID = 989
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 991 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.527' WHERE PROPTB_TUSS_ID = 990
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 992 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.547' WHERE PROPTB_TUSS_ID = 991
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 993 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.570' WHERE PROPTB_TUSS_ID = 992
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 994 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.590' WHERE PROPTB_TUSS_ID = 993
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 995 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.610' WHERE PROPTB_TUSS_ID = 994
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 996 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.630' WHERE PROPTB_TUSS_ID = 995
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 997 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.653' WHERE PROPTB_TUSS_ID = 996
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 998 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.683' WHERE PROPTB_TUSS_ID = 997
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 999 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.703' WHERE PROPTB_TUSS_ID = 998
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1000 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.723' WHERE PROPTB_TUSS_ID = 999
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1001 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.743' WHERE PROPTB_TUSS_ID = 1000
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1002 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.763' WHERE PROPTB_TUSS_ID = 1001
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1003 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.783' WHERE PROPTB_TUSS_ID = 1002
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1004 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.807' WHERE PROPTB_TUSS_ID = 1003
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1005 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.827' WHERE PROPTB_TUSS_ID = 1004
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1006 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.850' WHERE PROPTB_TUSS_ID = 1005
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1007 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.870' WHERE PROPTB_TUSS_ID = 1006
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1008 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.890' WHERE PROPTB_TUSS_ID = 1007
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1009 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.913' WHERE PROPTB_TUSS_ID = 1008
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1010 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.933' WHERE PROPTB_TUSS_ID = 1009
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1011 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.953' WHERE PROPTB_TUSS_ID = 1010
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1012 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.973' WHERE PROPTB_TUSS_ID = 1011
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1013 , CLOUD_SYNC_DATE = '2014-05-06 17:35:39.997' WHERE PROPTB_TUSS_ID = 1012
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1014 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.017' WHERE PROPTB_TUSS_ID = 1013
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1015 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.037' WHERE PROPTB_TUSS_ID = 1014
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1016 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.057' WHERE PROPTB_TUSS_ID = 1015
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1017 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.083' WHERE PROPTB_TUSS_ID = 1016
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1018 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.103' WHERE PROPTB_TUSS_ID = 1017
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1019 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.123' WHERE PROPTB_TUSS_ID = 1018
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1020 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.147' WHERE PROPTB_TUSS_ID = 1019
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1021 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.167' WHERE PROPTB_TUSS_ID = 1020
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1022 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.187' WHERE PROPTB_TUSS_ID = 1021
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1023 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.207' WHERE PROPTB_TUSS_ID = 1022
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1024 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.230' WHERE PROPTB_TUSS_ID = 1023
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1025 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.250' WHERE PROPTB_TUSS_ID = 1024
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1026 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.273' WHERE PROPTB_TUSS_ID = 1025
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1027 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.297' WHERE PROPTB_TUSS_ID = 1026
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1028 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.317' WHERE PROPTB_TUSS_ID = 1027
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1029 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.337' WHERE PROPTB_TUSS_ID = 1028
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1030 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.357' WHERE PROPTB_TUSS_ID = 1029
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1031 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.380' WHERE PROPTB_TUSS_ID = 1030
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1032 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.400' WHERE PROPTB_TUSS_ID = 1031
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1033 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.423' WHERE PROPTB_TUSS_ID = 1032
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1034 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.443' WHERE PROPTB_TUSS_ID = 1033
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1035 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.467' WHERE PROPTB_TUSS_ID = 1034
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1036 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.490' WHERE PROPTB_TUSS_ID = 1035
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1037 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.510' WHERE PROPTB_TUSS_ID = 1036
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1038 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.530' WHERE PROPTB_TUSS_ID = 1037
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1039 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.553' WHERE PROPTB_TUSS_ID = 1038
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1040 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.573' WHERE PROPTB_TUSS_ID = 1039
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1041 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.597' WHERE PROPTB_TUSS_ID = 1040
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1042 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.617' WHERE PROPTB_TUSS_ID = 1041
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1043 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.637' WHERE PROPTB_TUSS_ID = 1042
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1044 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.663' WHERE PROPTB_TUSS_ID = 1043
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1045 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.687' WHERE PROPTB_TUSS_ID = 1044
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1046 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.707' WHERE PROPTB_TUSS_ID = 1045
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1047 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.727' WHERE PROPTB_TUSS_ID = 1046
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1048 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.750' WHERE PROPTB_TUSS_ID = 1047
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1049 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.770' WHERE PROPTB_TUSS_ID = 1048
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1050 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.793' WHERE PROPTB_TUSS_ID = 1049
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1051 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.813' WHERE PROPTB_TUSS_ID = 1050
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1052 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.837' WHERE PROPTB_TUSS_ID = 1051
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1053 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.860' WHERE PROPTB_TUSS_ID = 1052
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1054 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.880' WHERE PROPTB_TUSS_ID = 1053
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1055 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.903' WHERE PROPTB_TUSS_ID = 1054
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1056 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.923' WHERE PROPTB_TUSS_ID = 1055
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1057 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.947' WHERE PROPTB_TUSS_ID = 1056
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1058 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.967' WHERE PROPTB_TUSS_ID = 1057
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1059 , CLOUD_SYNC_DATE = '2014-05-06 17:35:40.990' WHERE PROPTB_TUSS_ID = 1058
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1060 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.013' WHERE PROPTB_TUSS_ID = 1059
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1061 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.033' WHERE PROPTB_TUSS_ID = 1060
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1062 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.057' WHERE PROPTB_TUSS_ID = 1061
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1063 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.080' WHERE PROPTB_TUSS_ID = 1062
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1064 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.100' WHERE PROPTB_TUSS_ID = 1063
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1065 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.123' WHERE PROPTB_TUSS_ID = 1064
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1066 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.147' WHERE PROPTB_TUSS_ID = 1065
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1067 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.170' WHERE PROPTB_TUSS_ID = 1066
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1068 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.193' WHERE PROPTB_TUSS_ID = 1067
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1069 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.213' WHERE PROPTB_TUSS_ID = 1068
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1070 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.237' WHERE PROPTB_TUSS_ID = 1069
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1071 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.260' WHERE PROPTB_TUSS_ID = 1070
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1072 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.287' WHERE PROPTB_TUSS_ID = 1071
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1073 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.313' WHERE PROPTB_TUSS_ID = 1072
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1074 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.337' WHERE PROPTB_TUSS_ID = 1073
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1075 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.360' WHERE PROPTB_TUSS_ID = 1074
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1076 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.383' WHERE PROPTB_TUSS_ID = 1075
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1077 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.407' WHERE PROPTB_TUSS_ID = 1076
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1078 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.427' WHERE PROPTB_TUSS_ID = 1077
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1079 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.453' WHERE PROPTB_TUSS_ID = 1078
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1080 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.473' WHERE PROPTB_TUSS_ID = 1079
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1081 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.497' WHERE PROPTB_TUSS_ID = 1080
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1082 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.517' WHERE PROPTB_TUSS_ID = 1081
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1083 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.540' WHERE PROPTB_TUSS_ID = 1082
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1084 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.563' WHERE PROPTB_TUSS_ID = 1083
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1085 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.590' WHERE PROPTB_TUSS_ID = 1084
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1086 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.610' WHERE PROPTB_TUSS_ID = 1085
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1087 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.633' WHERE PROPTB_TUSS_ID = 1086
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1088 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.657' WHERE PROPTB_TUSS_ID = 1087
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1089 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.680' WHERE PROPTB_TUSS_ID = 1088
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1090 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.707' WHERE PROPTB_TUSS_ID = 1089
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1091 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.727' WHERE PROPTB_TUSS_ID = 1090
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1092 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.750' WHERE PROPTB_TUSS_ID = 1091
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1093 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.773' WHERE PROPTB_TUSS_ID = 1092
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1094 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.797' WHERE PROPTB_TUSS_ID = 1093
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1095 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.817' WHERE PROPTB_TUSS_ID = 1094
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1096 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.840' WHERE PROPTB_TUSS_ID = 1095
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1097 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.863' WHERE PROPTB_TUSS_ID = 1096
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1098 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.887' WHERE PROPTB_TUSS_ID = 1097
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1099 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.910' WHERE PROPTB_TUSS_ID = 1098
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1100 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.933' WHERE PROPTB_TUSS_ID = 1099
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1101 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.957' WHERE PROPTB_TUSS_ID = 1100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1102 , CLOUD_SYNC_DATE = '2014-05-06 17:35:41.980' WHERE PROPTB_TUSS_ID = 1101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1103 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.003' WHERE PROPTB_TUSS_ID = 1102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1104 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.027' WHERE PROPTB_TUSS_ID = 1103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1105 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.047' WHERE PROPTB_TUSS_ID = 1104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1106 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.070' WHERE PROPTB_TUSS_ID = 1105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1107 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.097' WHERE PROPTB_TUSS_ID = 1106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1108 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.117' WHERE PROPTB_TUSS_ID = 1107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1109 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.140' WHERE PROPTB_TUSS_ID = 1108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1110 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.163' WHERE PROPTB_TUSS_ID = 1109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1111 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.187' WHERE PROPTB_TUSS_ID = 1110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1112 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.210' WHERE PROPTB_TUSS_ID = 1111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1113 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.233' WHERE PROPTB_TUSS_ID = 1112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1114 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.257' WHERE PROPTB_TUSS_ID = 1113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1115 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.277' WHERE PROPTB_TUSS_ID = 1114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1116 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.300' WHERE PROPTB_TUSS_ID = 1115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1117 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.327' WHERE PROPTB_TUSS_ID = 1116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1118 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.350' WHERE PROPTB_TUSS_ID = 1117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1119 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.370' WHERE PROPTB_TUSS_ID = 1118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1120 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.393' WHERE PROPTB_TUSS_ID = 1119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1121 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.417' WHERE PROPTB_TUSS_ID = 1120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1122 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.443' WHERE PROPTB_TUSS_ID = 1121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1123 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.467' WHERE PROPTB_TUSS_ID = 1122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1124 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.490' WHERE PROPTB_TUSS_ID = 1123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1125 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.510' WHERE PROPTB_TUSS_ID = 1124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1126 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.533' WHERE PROPTB_TUSS_ID = 1125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1127 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.560' WHERE PROPTB_TUSS_ID = 1126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1128 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.583' WHERE PROPTB_TUSS_ID = 1127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1129 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.607' WHERE PROPTB_TUSS_ID = 1128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1130 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.630' WHERE PROPTB_TUSS_ID = 1129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1131 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.657' WHERE PROPTB_TUSS_ID = 1130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1132 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.683' WHERE PROPTB_TUSS_ID = 1131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1133 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.707' WHERE PROPTB_TUSS_ID = 1132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1134 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.727' WHERE PROPTB_TUSS_ID = 1133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1135 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.750' WHERE PROPTB_TUSS_ID = 1134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1136 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.777' WHERE PROPTB_TUSS_ID = 1135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1137 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.800' WHERE PROPTB_TUSS_ID = 1136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1138 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.823' WHERE PROPTB_TUSS_ID = 1137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1139 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.847' WHERE PROPTB_TUSS_ID = 1138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1140 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.870' WHERE PROPTB_TUSS_ID = 1139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1141 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.893' WHERE PROPTB_TUSS_ID = 1140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1142 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.917' WHERE PROPTB_TUSS_ID = 1141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1143 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.940' WHERE PROPTB_TUSS_ID = 1142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1144 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.967' WHERE PROPTB_TUSS_ID = 1143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1145 , CLOUD_SYNC_DATE = '2014-05-06 17:35:42.990' WHERE PROPTB_TUSS_ID = 1144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1146 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.013' WHERE PROPTB_TUSS_ID = 1145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1147 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.037' WHERE PROPTB_TUSS_ID = 1146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1148 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.060' WHERE PROPTB_TUSS_ID = 1147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1149 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.087' WHERE PROPTB_TUSS_ID = 1148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1150 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.110' WHERE PROPTB_TUSS_ID = 1149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1151 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.133' WHERE PROPTB_TUSS_ID = 1150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1152 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.157' WHERE PROPTB_TUSS_ID = 1151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1153 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.183' WHERE PROPTB_TUSS_ID = 1152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1154 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.207' WHERE PROPTB_TUSS_ID = 1153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1155 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.230' WHERE PROPTB_TUSS_ID = 1154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1156 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.253' WHERE PROPTB_TUSS_ID = 1155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1157 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.287' WHERE PROPTB_TUSS_ID = 1156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1158 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.310' WHERE PROPTB_TUSS_ID = 1157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1159 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.333' WHERE PROPTB_TUSS_ID = 1158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1160 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.357' WHERE PROPTB_TUSS_ID = 1159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1161 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.383' WHERE PROPTB_TUSS_ID = 1160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1162 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.407' WHERE PROPTB_TUSS_ID = 1161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1163 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.430' WHERE PROPTB_TUSS_ID = 1162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1164 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.453' WHERE PROPTB_TUSS_ID = 1163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1165 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.480' WHERE PROPTB_TUSS_ID = 1164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1166 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.503' WHERE PROPTB_TUSS_ID = 1165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1167 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.527' WHERE PROPTB_TUSS_ID = 1166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1168 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.550' WHERE PROPTB_TUSS_ID = 1167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1169 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.577' WHERE PROPTB_TUSS_ID = 1168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1170 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.600' WHERE PROPTB_TUSS_ID = 1169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1171 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.623' WHERE PROPTB_TUSS_ID = 1170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1172 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.650' WHERE PROPTB_TUSS_ID = 1171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1173 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.677' WHERE PROPTB_TUSS_ID = 1172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1174 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.703' WHERE PROPTB_TUSS_ID = 1173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1175 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.727' WHERE PROPTB_TUSS_ID = 1174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1176 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.753' WHERE PROPTB_TUSS_ID = 1175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1177 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.777' WHERE PROPTB_TUSS_ID = 1176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1178 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.800' WHERE PROPTB_TUSS_ID = 1177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1179 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.827' WHERE PROPTB_TUSS_ID = 1178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1180 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.850' WHERE PROPTB_TUSS_ID = 1179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1181 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.877' WHERE PROPTB_TUSS_ID = 1180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1182 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.900' WHERE PROPTB_TUSS_ID = 1181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1183 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.923' WHERE PROPTB_TUSS_ID = 1182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1184 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.950' WHERE PROPTB_TUSS_ID = 1183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1185 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.973' WHERE PROPTB_TUSS_ID = 1184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1186 , CLOUD_SYNC_DATE = '2014-05-06 17:35:43.997' WHERE PROPTB_TUSS_ID = 1185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1187 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.023' WHERE PROPTB_TUSS_ID = 1186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1188 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.050' WHERE PROPTB_TUSS_ID = 1187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1189 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.073' WHERE PROPTB_TUSS_ID = 1188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1190 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.097' WHERE PROPTB_TUSS_ID = 1189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1191 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.123' WHERE PROPTB_TUSS_ID = 1190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1192 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.147' WHERE PROPTB_TUSS_ID = 1191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1193 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.173' WHERE PROPTB_TUSS_ID = 1192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1194 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.197' WHERE PROPTB_TUSS_ID = 1193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1195 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.223' WHERE PROPTB_TUSS_ID = 1194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1196 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.247' WHERE PROPTB_TUSS_ID = 1195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1197 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.270' WHERE PROPTB_TUSS_ID = 1196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1198 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.297' WHERE PROPTB_TUSS_ID = 1197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1199 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.323' WHERE PROPTB_TUSS_ID = 1198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1200 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.347' WHERE PROPTB_TUSS_ID = 1199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1201 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.373' WHERE PROPTB_TUSS_ID = 1200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1202 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.397' WHERE PROPTB_TUSS_ID = 1201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1203 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.423' WHERE PROPTB_TUSS_ID = 1202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1204 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.447' WHERE PROPTB_TUSS_ID = 1203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1205 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.473' WHERE PROPTB_TUSS_ID = 1204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1206 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.497' WHERE PROPTB_TUSS_ID = 1205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1207 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.520' WHERE PROPTB_TUSS_ID = 1206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1208 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.547' WHERE PROPTB_TUSS_ID = 1207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1209 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.573' WHERE PROPTB_TUSS_ID = 1208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1210 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.597' WHERE PROPTB_TUSS_ID = 1209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1211 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.623' WHERE PROPTB_TUSS_ID = 1210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1212 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.647' WHERE PROPTB_TUSS_ID = 1211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1213 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.673' WHERE PROPTB_TUSS_ID = 1212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1214 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.700' WHERE PROPTB_TUSS_ID = 1213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1215 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.727' WHERE PROPTB_TUSS_ID = 1214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1216 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.750' WHERE PROPTB_TUSS_ID = 1215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1217 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.777' WHERE PROPTB_TUSS_ID = 1216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1218 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.800' WHERE PROPTB_TUSS_ID = 1217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1219 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.827' WHERE PROPTB_TUSS_ID = 1218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1220 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.850' WHERE PROPTB_TUSS_ID = 1219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1221 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.877' WHERE PROPTB_TUSS_ID = 1220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1222 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.903' WHERE PROPTB_TUSS_ID = 1221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1223 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.927' WHERE PROPTB_TUSS_ID = 1222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1224 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.957' WHERE PROPTB_TUSS_ID = 1223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1225 , CLOUD_SYNC_DATE = '2014-05-06 17:35:44.983' WHERE PROPTB_TUSS_ID = 1224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1226 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.007' WHERE PROPTB_TUSS_ID = 1225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1227 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.037' WHERE PROPTB_TUSS_ID = 1226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1228 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.060' WHERE PROPTB_TUSS_ID = 1227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1229 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.087' WHERE PROPTB_TUSS_ID = 1228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1230 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.117' WHERE PROPTB_TUSS_ID = 1229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1231 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.140' WHERE PROPTB_TUSS_ID = 1230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1232 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.167' WHERE PROPTB_TUSS_ID = 1231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1233 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.197' WHERE PROPTB_TUSS_ID = 1232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1234 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.220' WHERE PROPTB_TUSS_ID = 1233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1235 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.247' WHERE PROPTB_TUSS_ID = 1234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1236 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.277' WHERE PROPTB_TUSS_ID = 1235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1237 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.303' WHERE PROPTB_TUSS_ID = 1236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1238 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.327' WHERE PROPTB_TUSS_ID = 1237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1239 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.357' WHERE PROPTB_TUSS_ID = 1238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1240 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.383' WHERE PROPTB_TUSS_ID = 1239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1241 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.410' WHERE PROPTB_TUSS_ID = 1240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1242 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.440' WHERE PROPTB_TUSS_ID = 1241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1243 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.467' WHERE PROPTB_TUSS_ID = 1242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1244 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.493' WHERE PROPTB_TUSS_ID = 1243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1245 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.523' WHERE PROPTB_TUSS_ID = 1244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1246 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.547' WHERE PROPTB_TUSS_ID = 1245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1247 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.577' WHERE PROPTB_TUSS_ID = 1246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1248 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.603' WHERE PROPTB_TUSS_ID = 1247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1249 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.630' WHERE PROPTB_TUSS_ID = 1248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1250 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.657' WHERE PROPTB_TUSS_ID = 1249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1251 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.687' WHERE PROPTB_TUSS_ID = 1250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1252 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.713' WHERE PROPTB_TUSS_ID = 1251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1253 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.740' WHERE PROPTB_TUSS_ID = 1252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1254 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.770' WHERE PROPTB_TUSS_ID = 1253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1255 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.797' WHERE PROPTB_TUSS_ID = 1254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1256 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.823' WHERE PROPTB_TUSS_ID = 1255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1257 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.850' WHERE PROPTB_TUSS_ID = 1256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1258 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.877' WHERE PROPTB_TUSS_ID = 1257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1259 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.903' WHERE PROPTB_TUSS_ID = 1258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1260 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.930' WHERE PROPTB_TUSS_ID = 1259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1261 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.957' WHERE PROPTB_TUSS_ID = 1260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1262 , CLOUD_SYNC_DATE = '2014-05-06 17:35:45.987' WHERE PROPTB_TUSS_ID = 1261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1263 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.010' WHERE PROPTB_TUSS_ID = 1262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1264 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.037' WHERE PROPTB_TUSS_ID = 1263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1265 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.067' WHERE PROPTB_TUSS_ID = 1264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1266 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.093' WHERE PROPTB_TUSS_ID = 1265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1267 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.117' WHERE PROPTB_TUSS_ID = 1266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1268 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.147' WHERE PROPTB_TUSS_ID = 1267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1269 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.173' WHERE PROPTB_TUSS_ID = 1268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1270 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.203' WHERE PROPTB_TUSS_ID = 1269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1271 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.230' WHERE PROPTB_TUSS_ID = 1270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1272 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.257' WHERE PROPTB_TUSS_ID = 1271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1273 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.287' WHERE PROPTB_TUSS_ID = 1272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1274 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.317' WHERE PROPTB_TUSS_ID = 1273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1275 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.340' WHERE PROPTB_TUSS_ID = 1274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1276 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.370' WHERE PROPTB_TUSS_ID = 1275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1277 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.397' WHERE PROPTB_TUSS_ID = 1276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1278 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.427' WHERE PROPTB_TUSS_ID = 1277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1279 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.453' WHERE PROPTB_TUSS_ID = 1278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1280 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.477' WHERE PROPTB_TUSS_ID = 1279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1281 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.507' WHERE PROPTB_TUSS_ID = 1280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1282 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.533' WHERE PROPTB_TUSS_ID = 1281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1283 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.563' WHERE PROPTB_TUSS_ID = 1282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1284 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.590' WHERE PROPTB_TUSS_ID = 1283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1285 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.617' WHERE PROPTB_TUSS_ID = 1284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1286 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.647' WHERE PROPTB_TUSS_ID = 1285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1287 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.677' WHERE PROPTB_TUSS_ID = 1286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1288 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.703' WHERE PROPTB_TUSS_ID = 1287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1289 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.733' WHERE PROPTB_TUSS_ID = 1288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1290 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.757' WHERE PROPTB_TUSS_ID = 1289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1291 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.787' WHERE PROPTB_TUSS_ID = 1290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1292 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.813' WHERE PROPTB_TUSS_ID = 1291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1293 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.837' WHERE PROPTB_TUSS_ID = 1292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1294 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.867' WHERE PROPTB_TUSS_ID = 1293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1295 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.897' WHERE PROPTB_TUSS_ID = 1294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1296 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.927' WHERE PROPTB_TUSS_ID = 1295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1297 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.953' WHERE PROPTB_TUSS_ID = 1296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1298 , CLOUD_SYNC_DATE = '2014-05-06 17:35:46.980' WHERE PROPTB_TUSS_ID = 1297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1299 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.007' WHERE PROPTB_TUSS_ID = 1298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1300 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.033' WHERE PROPTB_TUSS_ID = 1299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1301 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.063' WHERE PROPTB_TUSS_ID = 1300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1302 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.090' WHERE PROPTB_TUSS_ID = 1301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1303 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.117' WHERE PROPTB_TUSS_ID = 1302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1304 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.147' WHERE PROPTB_TUSS_ID = 1303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1305 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.173' WHERE PROPTB_TUSS_ID = 1304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1306 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.203' WHERE PROPTB_TUSS_ID = 1305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1307 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.230' WHERE PROPTB_TUSS_ID = 1306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1308 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.257' WHERE PROPTB_TUSS_ID = 1307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1309 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.287' WHERE PROPTB_TUSS_ID = 1308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1310 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.317' WHERE PROPTB_TUSS_ID = 1309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1311 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.343' WHERE PROPTB_TUSS_ID = 1310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1312 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.370' WHERE PROPTB_TUSS_ID = 1311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1313 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.397' WHERE PROPTB_TUSS_ID = 1312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1314 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.427' WHERE PROPTB_TUSS_ID = 1313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1315 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.457' WHERE PROPTB_TUSS_ID = 1314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1316 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.487' WHERE PROPTB_TUSS_ID = 1315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1317 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.513' WHERE PROPTB_TUSS_ID = 1316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1318 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.540' WHERE PROPTB_TUSS_ID = 1317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1319 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.567' WHERE PROPTB_TUSS_ID = 1318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1320 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.597' WHERE PROPTB_TUSS_ID = 1319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1321 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.627' WHERE PROPTB_TUSS_ID = 1320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1322 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.657' WHERE PROPTB_TUSS_ID = 1321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1323 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.690' WHERE PROPTB_TUSS_ID = 1322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1324 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.717' WHERE PROPTB_TUSS_ID = 1323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1325 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.743' WHERE PROPTB_TUSS_ID = 1324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1326 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.773' WHERE PROPTB_TUSS_ID = 1325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1327 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.803' WHERE PROPTB_TUSS_ID = 1326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1328 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.830' WHERE PROPTB_TUSS_ID = 1327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1329 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.860' WHERE PROPTB_TUSS_ID = 1328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1330 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.893' WHERE PROPTB_TUSS_ID = 1329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1331 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.937' WHERE PROPTB_TUSS_ID = 1330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1332 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.970' WHERE PROPTB_TUSS_ID = 1331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1333 , CLOUD_SYNC_DATE = '2014-05-06 17:35:47.997' WHERE PROPTB_TUSS_ID = 1332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1334 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.027' WHERE PROPTB_TUSS_ID = 1333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1335 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.063' WHERE PROPTB_TUSS_ID = 1334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1336 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.093' WHERE PROPTB_TUSS_ID = 1335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1337 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.127' WHERE PROPTB_TUSS_ID = 1336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1338 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.153' WHERE PROPTB_TUSS_ID = 1337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1339 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.187' WHERE PROPTB_TUSS_ID = 1338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1340 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.213' WHERE PROPTB_TUSS_ID = 1339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1341 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.243' WHERE PROPTB_TUSS_ID = 1340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1342 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.270' WHERE PROPTB_TUSS_ID = 1341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1343 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.300' WHERE PROPTB_TUSS_ID = 1342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1344 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.327' WHERE PROPTB_TUSS_ID = 1343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1345 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.353' WHERE PROPTB_TUSS_ID = 1344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1346 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.383' WHERE PROPTB_TUSS_ID = 1345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1347 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.410' WHERE PROPTB_TUSS_ID = 1346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1348 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.440' WHERE PROPTB_TUSS_ID = 1347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1349 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.467' WHERE PROPTB_TUSS_ID = 1348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1350 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.497' WHERE PROPTB_TUSS_ID = 1349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1351 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.527' WHERE PROPTB_TUSS_ID = 1350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1352 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.557' WHERE PROPTB_TUSS_ID = 1351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1353 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.583' WHERE PROPTB_TUSS_ID = 1352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1354 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.610' WHERE PROPTB_TUSS_ID = 1353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1355 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.640' WHERE PROPTB_TUSS_ID = 1354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1356 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.670' WHERE PROPTB_TUSS_ID = 1355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1357 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.700' WHERE PROPTB_TUSS_ID = 1356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1358 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.727' WHERE PROPTB_TUSS_ID = 1357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1359 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.757' WHERE PROPTB_TUSS_ID = 1358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1360 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.783' WHERE PROPTB_TUSS_ID = 1359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1361 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.813' WHERE PROPTB_TUSS_ID = 1360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1362 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.843' WHERE PROPTB_TUSS_ID = 1361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1363 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.873' WHERE PROPTB_TUSS_ID = 1362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1364 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.903' WHERE PROPTB_TUSS_ID = 1363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1365 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.933' WHERE PROPTB_TUSS_ID = 1364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1366 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.960' WHERE PROPTB_TUSS_ID = 1365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1367 , CLOUD_SYNC_DATE = '2014-05-06 17:35:48.990' WHERE PROPTB_TUSS_ID = 1366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1368 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.017' WHERE PROPTB_TUSS_ID = 1367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1369 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.047' WHERE PROPTB_TUSS_ID = 1368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1370 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.077' WHERE PROPTB_TUSS_ID = 1369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1371 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.103' WHERE PROPTB_TUSS_ID = 1370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1372 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.133' WHERE PROPTB_TUSS_ID = 1371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1373 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.160' WHERE PROPTB_TUSS_ID = 1372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1374 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.190' WHERE PROPTB_TUSS_ID = 1373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1375 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.220' WHERE PROPTB_TUSS_ID = 1374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1376 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.247' WHERE PROPTB_TUSS_ID = 1375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1377 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.277' WHERE PROPTB_TUSS_ID = 1376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1378 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.307' WHERE PROPTB_TUSS_ID = 1377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1379 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.333' WHERE PROPTB_TUSS_ID = 1378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1380 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.363' WHERE PROPTB_TUSS_ID = 1379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1381 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.390' WHERE PROPTB_TUSS_ID = 1380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1382 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.420' WHERE PROPTB_TUSS_ID = 1381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1383 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.450' WHERE PROPTB_TUSS_ID = 1382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1384 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.480' WHERE PROPTB_TUSS_ID = 1383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1385 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.510' WHERE PROPTB_TUSS_ID = 1384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1386 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.540' WHERE PROPTB_TUSS_ID = 1385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1387 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.567' WHERE PROPTB_TUSS_ID = 1386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1388 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.600' WHERE PROPTB_TUSS_ID = 1387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1389 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.627' WHERE PROPTB_TUSS_ID = 1388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1390 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.660' WHERE PROPTB_TUSS_ID = 1389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1391 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.687' WHERE PROPTB_TUSS_ID = 1390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1392 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.720' WHERE PROPTB_TUSS_ID = 1391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1393 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.747' WHERE PROPTB_TUSS_ID = 1392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1394 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.777' WHERE PROPTB_TUSS_ID = 1393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1395 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.807' WHERE PROPTB_TUSS_ID = 1394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1396 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.837' WHERE PROPTB_TUSS_ID = 1395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1397 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.863' WHERE PROPTB_TUSS_ID = 1396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1398 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.897' WHERE PROPTB_TUSS_ID = 1397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1399 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.923' WHERE PROPTB_TUSS_ID = 1398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1400 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.957' WHERE PROPTB_TUSS_ID = 1399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1401 , CLOUD_SYNC_DATE = '2014-05-06 17:35:49.983' WHERE PROPTB_TUSS_ID = 1400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1402 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.013' WHERE PROPTB_TUSS_ID = 1401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1403 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.043' WHERE PROPTB_TUSS_ID = 1402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1404 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.073' WHERE PROPTB_TUSS_ID = 1403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1405 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.103' WHERE PROPTB_TUSS_ID = 1404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1406 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.140' WHERE PROPTB_TUSS_ID = 1405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1407 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.170' WHERE PROPTB_TUSS_ID = 1406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1408 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.200' WHERE PROPTB_TUSS_ID = 1407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1409 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.230' WHERE PROPTB_TUSS_ID = 1408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1410 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.260' WHERE PROPTB_TUSS_ID = 1409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1411 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.287' WHERE PROPTB_TUSS_ID = 1410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1412 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.320' WHERE PROPTB_TUSS_ID = 1411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1413 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.347' WHERE PROPTB_TUSS_ID = 1412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1414 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.377' WHERE PROPTB_TUSS_ID = 1413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1415 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.410' WHERE PROPTB_TUSS_ID = 1414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1416 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.437' WHERE PROPTB_TUSS_ID = 1415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1417 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.470' WHERE PROPTB_TUSS_ID = 1416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1418 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.497' WHERE PROPTB_TUSS_ID = 1417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1419 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.530' WHERE PROPTB_TUSS_ID = 1418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1420 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.557' WHERE PROPTB_TUSS_ID = 1419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1421 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.590' WHERE PROPTB_TUSS_ID = 1420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1422 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.617' WHERE PROPTB_TUSS_ID = 1421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1423 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.650' WHERE PROPTB_TUSS_ID = 1422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1424 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.683' WHERE PROPTB_TUSS_ID = 1423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1425 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.713' WHERE PROPTB_TUSS_ID = 1424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1426 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.743' WHERE PROPTB_TUSS_ID = 1425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1427 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.773' WHERE PROPTB_TUSS_ID = 1426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1428 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.800' WHERE PROPTB_TUSS_ID = 1427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1429 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.833' WHERE PROPTB_TUSS_ID = 1428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1430 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.863' WHERE PROPTB_TUSS_ID = 1429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1431 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.890' WHERE PROPTB_TUSS_ID = 1430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1432 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.923' WHERE PROPTB_TUSS_ID = 1431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1433 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.953' WHERE PROPTB_TUSS_ID = 1432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1434 , CLOUD_SYNC_DATE = '2014-05-06 17:35:50.983' WHERE PROPTB_TUSS_ID = 1433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1435 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.013' WHERE PROPTB_TUSS_ID = 1434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1436 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.043' WHERE PROPTB_TUSS_ID = 1435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1437 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.070' WHERE PROPTB_TUSS_ID = 1436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1438 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.103' WHERE PROPTB_TUSS_ID = 1437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1439 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.130' WHERE PROPTB_TUSS_ID = 1438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1440 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.163' WHERE PROPTB_TUSS_ID = 1439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1441 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.193' WHERE PROPTB_TUSS_ID = 1440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1442 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.223' WHERE PROPTB_TUSS_ID = 1441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1443 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.253' WHERE PROPTB_TUSS_ID = 1442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1444 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.283' WHERE PROPTB_TUSS_ID = 1443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1445 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.313' WHERE PROPTB_TUSS_ID = 1444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1446 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.343' WHERE PROPTB_TUSS_ID = 1445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1447 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.383' WHERE PROPTB_TUSS_ID = 1446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1448 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.417' WHERE PROPTB_TUSS_ID = 1447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1449 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.450' WHERE PROPTB_TUSS_ID = 1448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1450 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.483' WHERE PROPTB_TUSS_ID = 1449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1451 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.510' WHERE PROPTB_TUSS_ID = 1450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1452 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.543' WHERE PROPTB_TUSS_ID = 1451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1453 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.570' WHERE PROPTB_TUSS_ID = 1452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1454 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.603' WHERE PROPTB_TUSS_ID = 1453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1455 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.633' WHERE PROPTB_TUSS_ID = 1454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1456 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.667' WHERE PROPTB_TUSS_ID = 1455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1457 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.697' WHERE PROPTB_TUSS_ID = 1456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1458 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.727' WHERE PROPTB_TUSS_ID = 1457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1459 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.760' WHERE PROPTB_TUSS_ID = 1458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1460 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.790' WHERE PROPTB_TUSS_ID = 1459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1461 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.820' WHERE PROPTB_TUSS_ID = 1460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1462 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.850' WHERE PROPTB_TUSS_ID = 1461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1463 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.883' WHERE PROPTB_TUSS_ID = 1462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1464 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.917' WHERE PROPTB_TUSS_ID = 1463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1465 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.957' WHERE PROPTB_TUSS_ID = 1464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1466 , CLOUD_SYNC_DATE = '2014-05-06 17:35:51.990' WHERE PROPTB_TUSS_ID = 1465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1467 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.023' WHERE PROPTB_TUSS_ID = 1466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1468 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.057' WHERE PROPTB_TUSS_ID = 1467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1469 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.090' WHERE PROPTB_TUSS_ID = 1468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1470 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.123' WHERE PROPTB_TUSS_ID = 1469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1471 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.157' WHERE PROPTB_TUSS_ID = 1470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1472 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.187' WHERE PROPTB_TUSS_ID = 1471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1473 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.217' WHERE PROPTB_TUSS_ID = 1472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1474 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.247' WHERE PROPTB_TUSS_ID = 1473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1475 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.280' WHERE PROPTB_TUSS_ID = 1474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1476 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.310' WHERE PROPTB_TUSS_ID = 1475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1477 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.340' WHERE PROPTB_TUSS_ID = 1476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1478 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.373' WHERE PROPTB_TUSS_ID = 1477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1479 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.403' WHERE PROPTB_TUSS_ID = 1478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1480 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.437' WHERE PROPTB_TUSS_ID = 1479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1481 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.467' WHERE PROPTB_TUSS_ID = 1480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1482 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.497' WHERE PROPTB_TUSS_ID = 1481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1483 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.530' WHERE PROPTB_TUSS_ID = 1482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1484 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.560' WHERE PROPTB_TUSS_ID = 1483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1485 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.593' WHERE PROPTB_TUSS_ID = 1484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1486 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.627' WHERE PROPTB_TUSS_ID = 1485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1487 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.660' WHERE PROPTB_TUSS_ID = 1486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1488 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.693' WHERE PROPTB_TUSS_ID = 1487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1489 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.723' WHERE PROPTB_TUSS_ID = 1488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1490 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.757' WHERE PROPTB_TUSS_ID = 1489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1491 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.787' WHERE PROPTB_TUSS_ID = 1490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1492 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.820' WHERE PROPTB_TUSS_ID = 1491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1493 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.853' WHERE PROPTB_TUSS_ID = 1492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1494 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.883' WHERE PROPTB_TUSS_ID = 1493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1495 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.917' WHERE PROPTB_TUSS_ID = 1494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1496 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.947' WHERE PROPTB_TUSS_ID = 1495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1497 , CLOUD_SYNC_DATE = '2014-05-06 17:35:52.980' WHERE PROPTB_TUSS_ID = 1496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1498 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.010' WHERE PROPTB_TUSS_ID = 1497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1499 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.043' WHERE PROPTB_TUSS_ID = 1498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1500 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.073' WHERE PROPTB_TUSS_ID = 1499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1501 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.110' WHERE PROPTB_TUSS_ID = 1500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1502 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.143' WHERE PROPTB_TUSS_ID = 1501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1503 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.177' WHERE PROPTB_TUSS_ID = 1502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1504 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.207' WHERE PROPTB_TUSS_ID = 1503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1505 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.237' WHERE PROPTB_TUSS_ID = 1504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1506 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.270' WHERE PROPTB_TUSS_ID = 1505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1507 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.307' WHERE PROPTB_TUSS_ID = 1506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1508 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.340' WHERE PROPTB_TUSS_ID = 1507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1509 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.373' WHERE PROPTB_TUSS_ID = 1508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1510 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.403' WHERE PROPTB_TUSS_ID = 1509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1511 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.437' WHERE PROPTB_TUSS_ID = 1510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1512 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.470' WHERE PROPTB_TUSS_ID = 1511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1513 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.500' WHERE PROPTB_TUSS_ID = 1512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1514 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.533' WHERE PROPTB_TUSS_ID = 1513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1515 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.567' WHERE PROPTB_TUSS_ID = 1514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1516 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.597' WHERE PROPTB_TUSS_ID = 1515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1517 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.630' WHERE PROPTB_TUSS_ID = 1516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1518 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.663' WHERE PROPTB_TUSS_ID = 1517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1519 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.693' WHERE PROPTB_TUSS_ID = 1518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1520 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.727' WHERE PROPTB_TUSS_ID = 1519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1521 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.757' WHERE PROPTB_TUSS_ID = 1520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1522 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.790' WHERE PROPTB_TUSS_ID = 1521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1523 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.823' WHERE PROPTB_TUSS_ID = 1522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1524 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.853' WHERE PROPTB_TUSS_ID = 1523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1525 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.887' WHERE PROPTB_TUSS_ID = 1524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1526 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.920' WHERE PROPTB_TUSS_ID = 1525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1527 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.950' WHERE PROPTB_TUSS_ID = 1526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1528 , CLOUD_SYNC_DATE = '2014-05-06 17:35:53.983' WHERE PROPTB_TUSS_ID = 1527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1529 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.017' WHERE PROPTB_TUSS_ID = 1528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1530 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.047' WHERE PROPTB_TUSS_ID = 1529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1531 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.080' WHERE PROPTB_TUSS_ID = 1530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1532 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.110' WHERE PROPTB_TUSS_ID = 1531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1533 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.143' WHERE PROPTB_TUSS_ID = 1532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1534 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.177' WHERE PROPTB_TUSS_ID = 1533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1535 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.207' WHERE PROPTB_TUSS_ID = 1534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1536 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.240' WHERE PROPTB_TUSS_ID = 1535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1537 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.273' WHERE PROPTB_TUSS_ID = 1536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1538 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.303' WHERE PROPTB_TUSS_ID = 1537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1539 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.337' WHERE PROPTB_TUSS_ID = 1538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1540 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.370' WHERE PROPTB_TUSS_ID = 1539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1541 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.403' WHERE PROPTB_TUSS_ID = 1540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1542 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.437' WHERE PROPTB_TUSS_ID = 1541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1543 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.470' WHERE PROPTB_TUSS_ID = 1542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1544 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.500' WHERE PROPTB_TUSS_ID = 1543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1545 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.533' WHERE PROPTB_TUSS_ID = 1544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1546 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.567' WHERE PROPTB_TUSS_ID = 1545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1547 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.597' WHERE PROPTB_TUSS_ID = 1546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1548 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.633' WHERE PROPTB_TUSS_ID = 1547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1549 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.670' WHERE PROPTB_TUSS_ID = 1548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1550 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.707' WHERE PROPTB_TUSS_ID = 1549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1551 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.743' WHERE PROPTB_TUSS_ID = 1550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1552 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.787' WHERE PROPTB_TUSS_ID = 1551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1553 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.823' WHERE PROPTB_TUSS_ID = 1552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1554 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.860' WHERE PROPTB_TUSS_ID = 1553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1555 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.907' WHERE PROPTB_TUSS_ID = 1554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1556 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.937' WHERE PROPTB_TUSS_ID = 1555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1557 , CLOUD_SYNC_DATE = '2014-05-06 17:35:54.973' WHERE PROPTB_TUSS_ID = 1556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1558 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.007' WHERE PROPTB_TUSS_ID = 1557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1559 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.037' WHERE PROPTB_TUSS_ID = 1558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1560 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.070' WHERE PROPTB_TUSS_ID = 1559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1561 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.107' WHERE PROPTB_TUSS_ID = 1560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1562 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.140' WHERE PROPTB_TUSS_ID = 1561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1563 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.183' WHERE PROPTB_TUSS_ID = 1562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1564 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.240' WHERE PROPTB_TUSS_ID = 1563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1565 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.287' WHERE PROPTB_TUSS_ID = 1564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1566 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.320' WHERE PROPTB_TUSS_ID = 1565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1567 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.357' WHERE PROPTB_TUSS_ID = 1566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1568 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.387' WHERE PROPTB_TUSS_ID = 1567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1569 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.423' WHERE PROPTB_TUSS_ID = 1568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1570 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.453' WHERE PROPTB_TUSS_ID = 1569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1571 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.490' WHERE PROPTB_TUSS_ID = 1570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1572 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.523' WHERE PROPTB_TUSS_ID = 1571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1573 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.557' WHERE PROPTB_TUSS_ID = 1572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1574 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.590' WHERE PROPTB_TUSS_ID = 1573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1575 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.627' WHERE PROPTB_TUSS_ID = 1574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1576 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.657' WHERE PROPTB_TUSS_ID = 1575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1577 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.697' WHERE PROPTB_TUSS_ID = 1576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1578 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.730' WHERE PROPTB_TUSS_ID = 1577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1579 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.763' WHERE PROPTB_TUSS_ID = 1578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1580 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.797' WHERE PROPTB_TUSS_ID = 1579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1581 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.830' WHERE PROPTB_TUSS_ID = 1580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1582 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.863' WHERE PROPTB_TUSS_ID = 1581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1583 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.897' WHERE PROPTB_TUSS_ID = 1582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1584 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.930' WHERE PROPTB_TUSS_ID = 1583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1585 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.967' WHERE PROPTB_TUSS_ID = 1584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1586 , CLOUD_SYNC_DATE = '2014-05-06 17:35:55.997' WHERE PROPTB_TUSS_ID = 1585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1587 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.033' WHERE PROPTB_TUSS_ID = 1586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1588 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.067' WHERE PROPTB_TUSS_ID = 1587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1589 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.103' WHERE PROPTB_TUSS_ID = 1588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1590 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.133' WHERE PROPTB_TUSS_ID = 1589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1591 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.167' WHERE PROPTB_TUSS_ID = 1590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1592 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.203' WHERE PROPTB_TUSS_ID = 1591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1593 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.233' WHERE PROPTB_TUSS_ID = 1592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1594 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.267' WHERE PROPTB_TUSS_ID = 1593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1595 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.303' WHERE PROPTB_TUSS_ID = 1594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1596 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.337' WHERE PROPTB_TUSS_ID = 1595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1597 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.367' WHERE PROPTB_TUSS_ID = 1596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1598 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.403' WHERE PROPTB_TUSS_ID = 1597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1599 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.437' WHERE PROPTB_TUSS_ID = 1598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1600 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.470' WHERE PROPTB_TUSS_ID = 1599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1601 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.507' WHERE PROPTB_TUSS_ID = 1600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1602 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.540' WHERE PROPTB_TUSS_ID = 1601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1603 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.577' WHERE PROPTB_TUSS_ID = 1602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1604 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.607' WHERE PROPTB_TUSS_ID = 1603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1605 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.643' WHERE PROPTB_TUSS_ID = 1604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1606 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.677' WHERE PROPTB_TUSS_ID = 1605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1607 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.713' WHERE PROPTB_TUSS_ID = 1606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1608 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.743' WHERE PROPTB_TUSS_ID = 1607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1609 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.780' WHERE PROPTB_TUSS_ID = 1608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1610 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.813' WHERE PROPTB_TUSS_ID = 1609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1611 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.847' WHERE PROPTB_TUSS_ID = 1610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1612 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.880' WHERE PROPTB_TUSS_ID = 1611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1613 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.917' WHERE PROPTB_TUSS_ID = 1612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1614 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.950' WHERE PROPTB_TUSS_ID = 1613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1615 , CLOUD_SYNC_DATE = '2014-05-06 17:35:56.983' WHERE PROPTB_TUSS_ID = 1614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1616 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.017' WHERE PROPTB_TUSS_ID = 1615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1617 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.050' WHERE PROPTB_TUSS_ID = 1616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1618 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.087' WHERE PROPTB_TUSS_ID = 1617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1619 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.120' WHERE PROPTB_TUSS_ID = 1618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1620 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.153' WHERE PROPTB_TUSS_ID = 1619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1621 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.187' WHERE PROPTB_TUSS_ID = 1620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1622 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.223' WHERE PROPTB_TUSS_ID = 1621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1623 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.257' WHERE PROPTB_TUSS_ID = 1622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1624 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.290' WHERE PROPTB_TUSS_ID = 1623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1625 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.327' WHERE PROPTB_TUSS_ID = 1624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1626 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.360' WHERE PROPTB_TUSS_ID = 1625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1627 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.393' WHERE PROPTB_TUSS_ID = 1626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1628 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.430' WHERE PROPTB_TUSS_ID = 1627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1629 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.463' WHERE PROPTB_TUSS_ID = 1628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1630 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.500' WHERE PROPTB_TUSS_ID = 1629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1631 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.543' WHERE PROPTB_TUSS_ID = 1630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1632 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.583' WHERE PROPTB_TUSS_ID = 1631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1633 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.620' WHERE PROPTB_TUSS_ID = 1632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1634 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.657' WHERE PROPTB_TUSS_ID = 1633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1635 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.697' WHERE PROPTB_TUSS_ID = 1634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1636 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.730' WHERE PROPTB_TUSS_ID = 1635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1637 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.767' WHERE PROPTB_TUSS_ID = 1636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1638 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.803' WHERE PROPTB_TUSS_ID = 1637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1639 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.837' WHERE PROPTB_TUSS_ID = 1638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1640 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.870' WHERE PROPTB_TUSS_ID = 1639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1641 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.907' WHERE PROPTB_TUSS_ID = 1640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1642 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.940' WHERE PROPTB_TUSS_ID = 1641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1643 , CLOUD_SYNC_DATE = '2014-05-06 17:35:57.977' WHERE PROPTB_TUSS_ID = 1642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1644 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.007' WHERE PROPTB_TUSS_ID = 1643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1645 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.043' WHERE PROPTB_TUSS_ID = 1644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1646 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.080' WHERE PROPTB_TUSS_ID = 1645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1647 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.113' WHERE PROPTB_TUSS_ID = 1646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1648 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.150' WHERE PROPTB_TUSS_ID = 1647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1649 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.183' WHERE PROPTB_TUSS_ID = 1648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1650 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.217' WHERE PROPTB_TUSS_ID = 1649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1651 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.253' WHERE PROPTB_TUSS_ID = 1650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1652 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.287' WHERE PROPTB_TUSS_ID = 1651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1653 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.320' WHERE PROPTB_TUSS_ID = 1652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1654 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.357' WHERE PROPTB_TUSS_ID = 1653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1655 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.390' WHERE PROPTB_TUSS_ID = 1654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1656 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.427' WHERE PROPTB_TUSS_ID = 1655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1657 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.460' WHERE PROPTB_TUSS_ID = 1656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1658 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.497' WHERE PROPTB_TUSS_ID = 1657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1659 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.530' WHERE PROPTB_TUSS_ID = 1658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1660 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.567' WHERE PROPTB_TUSS_ID = 1659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1661 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.603' WHERE PROPTB_TUSS_ID = 1660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1662 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.637' WHERE PROPTB_TUSS_ID = 1661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1663 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.677' WHERE PROPTB_TUSS_ID = 1662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1664 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.710' WHERE PROPTB_TUSS_ID = 1663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1665 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.747' WHERE PROPTB_TUSS_ID = 1664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1666 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.783' WHERE PROPTB_TUSS_ID = 1665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1667 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.817' WHERE PROPTB_TUSS_ID = 1666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1668 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.853' WHERE PROPTB_TUSS_ID = 1667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1669 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.890' WHERE PROPTB_TUSS_ID = 1668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1670 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.927' WHERE PROPTB_TUSS_ID = 1669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1671 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.963' WHERE PROPTB_TUSS_ID = 1670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1672 , CLOUD_SYNC_DATE = '2014-05-06 17:35:58.997' WHERE PROPTB_TUSS_ID = 1671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1673 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.030' WHERE PROPTB_TUSS_ID = 1672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1674 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.067' WHERE PROPTB_TUSS_ID = 1673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1675 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.103' WHERE PROPTB_TUSS_ID = 1674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1676 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.147' WHERE PROPTB_TUSS_ID = 1675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1677 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.183' WHERE PROPTB_TUSS_ID = 1676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1678 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.227' WHERE PROPTB_TUSS_ID = 1677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1679 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.263' WHERE PROPTB_TUSS_ID = 1678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1680 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.300' WHERE PROPTB_TUSS_ID = 1679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1681 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.337' WHERE PROPTB_TUSS_ID = 1680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1682 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.373' WHERE PROPTB_TUSS_ID = 1681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1683 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.417' WHERE PROPTB_TUSS_ID = 1682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1684 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.463' WHERE PROPTB_TUSS_ID = 1683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1685 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.500' WHERE PROPTB_TUSS_ID = 1684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1686 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.537' WHERE PROPTB_TUSS_ID = 1685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1687 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.577' WHERE PROPTB_TUSS_ID = 1686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1688 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.613' WHERE PROPTB_TUSS_ID = 1687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1689 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.647' WHERE PROPTB_TUSS_ID = 1688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1690 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.687' WHERE PROPTB_TUSS_ID = 1689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1691 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.727' WHERE PROPTB_TUSS_ID = 1690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1692 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.763' WHERE PROPTB_TUSS_ID = 1691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1693 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.800' WHERE PROPTB_TUSS_ID = 1692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1694 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.840' WHERE PROPTB_TUSS_ID = 1693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1695 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.883' WHERE PROPTB_TUSS_ID = 1694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1696 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.923' WHERE PROPTB_TUSS_ID = 1695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1697 , CLOUD_SYNC_DATE = '2014-05-06 17:35:59.963' WHERE PROPTB_TUSS_ID = 1696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1698 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.000' WHERE PROPTB_TUSS_ID = 1697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1699 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.050' WHERE PROPTB_TUSS_ID = 1698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1700 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.087' WHERE PROPTB_TUSS_ID = 1699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1701 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.130' WHERE PROPTB_TUSS_ID = 1700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1702 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.167' WHERE PROPTB_TUSS_ID = 1701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1703 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.207' WHERE PROPTB_TUSS_ID = 1702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1704 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.243' WHERE PROPTB_TUSS_ID = 1703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1705 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.283' WHERE PROPTB_TUSS_ID = 1704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1706 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.320' WHERE PROPTB_TUSS_ID = 1705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1707 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.357' WHERE PROPTB_TUSS_ID = 1706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1708 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.400' WHERE PROPTB_TUSS_ID = 1707
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1709 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.447' WHERE PROPTB_TUSS_ID = 1708
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1710 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.487' WHERE PROPTB_TUSS_ID = 1709
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1711 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.527' WHERE PROPTB_TUSS_ID = 1710
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1712 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.567' WHERE PROPTB_TUSS_ID = 1711
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1713 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.603' WHERE PROPTB_TUSS_ID = 1712
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1714 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.637' WHERE PROPTB_TUSS_ID = 1713
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1715 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.677' WHERE PROPTB_TUSS_ID = 1714
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1716 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.713' WHERE PROPTB_TUSS_ID = 1715
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1717 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.750' WHERE PROPTB_TUSS_ID = 1716
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1718 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.787' WHERE PROPTB_TUSS_ID = 1717
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1719 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.827' WHERE PROPTB_TUSS_ID = 1718
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1720 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.860' WHERE PROPTB_TUSS_ID = 1719
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1721 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.897' WHERE PROPTB_TUSS_ID = 1720
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1722 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.933' WHERE PROPTB_TUSS_ID = 1721
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1723 , CLOUD_SYNC_DATE = '2014-05-06 17:36:00.970' WHERE PROPTB_TUSS_ID = 1722
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1724 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.007' WHERE PROPTB_TUSS_ID = 1723
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1725 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.043' WHERE PROPTB_TUSS_ID = 1724
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1726 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.083' WHERE PROPTB_TUSS_ID = 1725
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1727 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.127' WHERE PROPTB_TUSS_ID = 1726
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1728 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.170' WHERE PROPTB_TUSS_ID = 1727
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1729 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.207' WHERE PROPTB_TUSS_ID = 1728
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1730 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.247' WHERE PROPTB_TUSS_ID = 1729
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1731 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.280' WHERE PROPTB_TUSS_ID = 1730
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1732 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.320' WHERE PROPTB_TUSS_ID = 1731
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1733 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.357' WHERE PROPTB_TUSS_ID = 1732
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1734 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.393' WHERE PROPTB_TUSS_ID = 1733
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1735 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.427' WHERE PROPTB_TUSS_ID = 1734
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1736 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.473' WHERE PROPTB_TUSS_ID = 1735
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1737 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.517' WHERE PROPTB_TUSS_ID = 1736
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1738 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.557' WHERE PROPTB_TUSS_ID = 1737
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1739 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.593' WHERE PROPTB_TUSS_ID = 1738
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1740 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.630' WHERE PROPTB_TUSS_ID = 1739
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1741 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.670' WHERE PROPTB_TUSS_ID = 1740
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1742 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.707' WHERE PROPTB_TUSS_ID = 1741
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1743 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.740' WHERE PROPTB_TUSS_ID = 1742
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1744 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.783' WHERE PROPTB_TUSS_ID = 1743
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1745 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.827' WHERE PROPTB_TUSS_ID = 1744
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1746 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.870' WHERE PROPTB_TUSS_ID = 1745
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1747 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.913' WHERE PROPTB_TUSS_ID = 1746
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1748 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.950' WHERE PROPTB_TUSS_ID = 1747
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1749 , CLOUD_SYNC_DATE = '2014-05-06 17:36:01.990' WHERE PROPTB_TUSS_ID = 1748
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1750 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.030' WHERE PROPTB_TUSS_ID = 1749
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1751 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.070' WHERE PROPTB_TUSS_ID = 1750
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1752 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.103' WHERE PROPTB_TUSS_ID = 1751
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1753 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.140' WHERE PROPTB_TUSS_ID = 1752
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1754 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.177' WHERE PROPTB_TUSS_ID = 1753
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1755 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.217' WHERE PROPTB_TUSS_ID = 1754
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1756 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.253' WHERE PROPTB_TUSS_ID = 1755
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1757 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.290' WHERE PROPTB_TUSS_ID = 1756
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1758 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.327' WHERE PROPTB_TUSS_ID = 1757
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1759 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.367' WHERE PROPTB_TUSS_ID = 1758
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1760 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.403' WHERE PROPTB_TUSS_ID = 1759
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1761 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.440' WHERE PROPTB_TUSS_ID = 1760
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1762 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.477' WHERE PROPTB_TUSS_ID = 1761
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1763 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.517' WHERE PROPTB_TUSS_ID = 1762
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1764 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.553' WHERE PROPTB_TUSS_ID = 1763
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1765 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.593' WHERE PROPTB_TUSS_ID = 1764
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1766 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.630' WHERE PROPTB_TUSS_ID = 1765
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1767 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.670' WHERE PROPTB_TUSS_ID = 1766
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1768 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.707' WHERE PROPTB_TUSS_ID = 1767
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1769 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.747' WHERE PROPTB_TUSS_ID = 1768
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1770 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.787' WHERE PROPTB_TUSS_ID = 1769
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1771 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.823' WHERE PROPTB_TUSS_ID = 1770
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1772 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.860' WHERE PROPTB_TUSS_ID = 1771
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1773 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.900' WHERE PROPTB_TUSS_ID = 1772
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1774 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.940' WHERE PROPTB_TUSS_ID = 1773
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1775 , CLOUD_SYNC_DATE = '2014-05-06 17:36:02.977' WHERE PROPTB_TUSS_ID = 1774
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1776 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.017' WHERE PROPTB_TUSS_ID = 1775
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1777 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.057' WHERE PROPTB_TUSS_ID = 1776
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1778 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.097' WHERE PROPTB_TUSS_ID = 1777
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1779 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.137' WHERE PROPTB_TUSS_ID = 1778
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1780 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.173' WHERE PROPTB_TUSS_ID = 1779
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1781 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.213' WHERE PROPTB_TUSS_ID = 1780
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1782 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.253' WHERE PROPTB_TUSS_ID = 1781
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1783 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.300' WHERE PROPTB_TUSS_ID = 1782
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1784 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.337' WHERE PROPTB_TUSS_ID = 1783
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1785 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.373' WHERE PROPTB_TUSS_ID = 1784
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1786 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.417' WHERE PROPTB_TUSS_ID = 1785
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1787 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.453' WHERE PROPTB_TUSS_ID = 1786
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1788 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.493' WHERE PROPTB_TUSS_ID = 1787
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1789 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.533' WHERE PROPTB_TUSS_ID = 1788
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1790 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.573' WHERE PROPTB_TUSS_ID = 1789
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1791 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.613' WHERE PROPTB_TUSS_ID = 1790
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1792 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.660' WHERE PROPTB_TUSS_ID = 1791
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1793 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.700' WHERE PROPTB_TUSS_ID = 1792
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1794 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.737' WHERE PROPTB_TUSS_ID = 1793
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1795 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.777' WHERE PROPTB_TUSS_ID = 1794
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1796 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.813' WHERE PROPTB_TUSS_ID = 1795
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1797 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.850' WHERE PROPTB_TUSS_ID = 1796
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1798 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.887' WHERE PROPTB_TUSS_ID = 1797
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1799 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.927' WHERE PROPTB_TUSS_ID = 1798
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1800 , CLOUD_SYNC_DATE = '2014-05-06 17:36:03.967' WHERE PROPTB_TUSS_ID = 1799
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1801 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.003' WHERE PROPTB_TUSS_ID = 1800
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1802 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.047' WHERE PROPTB_TUSS_ID = 1801
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1803 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.083' WHERE PROPTB_TUSS_ID = 1802
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1804 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.123' WHERE PROPTB_TUSS_ID = 1803
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1805 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.160' WHERE PROPTB_TUSS_ID = 1804
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1806 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.197' WHERE PROPTB_TUSS_ID = 1805
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1807 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.237' WHERE PROPTB_TUSS_ID = 1806
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1808 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.277' WHERE PROPTB_TUSS_ID = 1807
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1809 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.313' WHERE PROPTB_TUSS_ID = 1808
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1810 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.350' WHERE PROPTB_TUSS_ID = 1809
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1811 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.390' WHERE PROPTB_TUSS_ID = 1810
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1812 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.427' WHERE PROPTB_TUSS_ID = 1811
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1813 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.467' WHERE PROPTB_TUSS_ID = 1812
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1814 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.507' WHERE PROPTB_TUSS_ID = 1813
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1815 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.543' WHERE PROPTB_TUSS_ID = 1814
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1816 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.583' WHERE PROPTB_TUSS_ID = 1815
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1817 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.620' WHERE PROPTB_TUSS_ID = 1816
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1818 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.660' WHERE PROPTB_TUSS_ID = 1817
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1819 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.697' WHERE PROPTB_TUSS_ID = 1818
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1820 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.737' WHERE PROPTB_TUSS_ID = 1819
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1821 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.777' WHERE PROPTB_TUSS_ID = 1820
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1822 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.813' WHERE PROPTB_TUSS_ID = 1821
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1823 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.853' WHERE PROPTB_TUSS_ID = 1822
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1824 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.893' WHERE PROPTB_TUSS_ID = 1823
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1825 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.933' WHERE PROPTB_TUSS_ID = 1824
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1826 , CLOUD_SYNC_DATE = '2014-05-06 17:36:04.970' WHERE PROPTB_TUSS_ID = 1825
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1827 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.010' WHERE PROPTB_TUSS_ID = 1826
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1828 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.050' WHERE PROPTB_TUSS_ID = 1827
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1829 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.090' WHERE PROPTB_TUSS_ID = 1828
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1830 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.130' WHERE PROPTB_TUSS_ID = 1829
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1831 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.170' WHERE PROPTB_TUSS_ID = 1830
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1832 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.213' WHERE PROPTB_TUSS_ID = 1831
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1833 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.253' WHERE PROPTB_TUSS_ID = 1832
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1834 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.293' WHERE PROPTB_TUSS_ID = 1833
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1835 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.333' WHERE PROPTB_TUSS_ID = 1834
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1836 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.377' WHERE PROPTB_TUSS_ID = 1835
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1837 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.417' WHERE PROPTB_TUSS_ID = 1836
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1838 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.453' WHERE PROPTB_TUSS_ID = 1837
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1839 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.493' WHERE PROPTB_TUSS_ID = 1838
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1840 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.533' WHERE PROPTB_TUSS_ID = 1839
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1841 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.573' WHERE PROPTB_TUSS_ID = 1840
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1842 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.613' WHERE PROPTB_TUSS_ID = 1841
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1843 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.653' WHERE PROPTB_TUSS_ID = 1842
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1844 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.697' WHERE PROPTB_TUSS_ID = 1843
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1845 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.740' WHERE PROPTB_TUSS_ID = 1844
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1846 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.780' WHERE PROPTB_TUSS_ID = 1845
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1847 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.820' WHERE PROPTB_TUSS_ID = 1846
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1848 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.863' WHERE PROPTB_TUSS_ID = 1847
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1849 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.907' WHERE PROPTB_TUSS_ID = 1848
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1850 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.950' WHERE PROPTB_TUSS_ID = 1849
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1851 , CLOUD_SYNC_DATE = '2014-05-06 17:36:05.990' WHERE PROPTB_TUSS_ID = 1850
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1852 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.033' WHERE PROPTB_TUSS_ID = 1851
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1853 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.073' WHERE PROPTB_TUSS_ID = 1852
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1854 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.117' WHERE PROPTB_TUSS_ID = 1853
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1855 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.153' WHERE PROPTB_TUSS_ID = 1854
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1856 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.193' WHERE PROPTB_TUSS_ID = 1855
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1857 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.237' WHERE PROPTB_TUSS_ID = 1856
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1858 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.277' WHERE PROPTB_TUSS_ID = 1857
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1859 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.317' WHERE PROPTB_TUSS_ID = 1858
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1860 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.357' WHERE PROPTB_TUSS_ID = 1859
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1861 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.393' WHERE PROPTB_TUSS_ID = 1860
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1862 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.437' WHERE PROPTB_TUSS_ID = 1861
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1863 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.477' WHERE PROPTB_TUSS_ID = 1862
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1864 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.517' WHERE PROPTB_TUSS_ID = 1863
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1865 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.560' WHERE PROPTB_TUSS_ID = 1864
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1866 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.600' WHERE PROPTB_TUSS_ID = 1865
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1867 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.640' WHERE PROPTB_TUSS_ID = 1866
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1868 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.687' WHERE PROPTB_TUSS_ID = 1867
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1869 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.727' WHERE PROPTB_TUSS_ID = 1868
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1870 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.767' WHERE PROPTB_TUSS_ID = 1869
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1871 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.807' WHERE PROPTB_TUSS_ID = 1870
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1872 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.850' WHERE PROPTB_TUSS_ID = 1871
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1873 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.890' WHERE PROPTB_TUSS_ID = 1872
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1874 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.927' WHERE PROPTB_TUSS_ID = 1873
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1875 , CLOUD_SYNC_DATE = '2014-05-06 17:36:06.967' WHERE PROPTB_TUSS_ID = 1874
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1876 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.007' WHERE PROPTB_TUSS_ID = 1875
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1877 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.047' WHERE PROPTB_TUSS_ID = 1876
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1878 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.087' WHERE PROPTB_TUSS_ID = 1877
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1879 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.130' WHERE PROPTB_TUSS_ID = 1878
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1880 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.170' WHERE PROPTB_TUSS_ID = 1879
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1881 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.210' WHERE PROPTB_TUSS_ID = 1880
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1882 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.247' WHERE PROPTB_TUSS_ID = 1881
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1883 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.287' WHERE PROPTB_TUSS_ID = 1882
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1884 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.327' WHERE PROPTB_TUSS_ID = 1883
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1885 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.367' WHERE PROPTB_TUSS_ID = 1884
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1886 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.407' WHERE PROPTB_TUSS_ID = 1885
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1887 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.447' WHERE PROPTB_TUSS_ID = 1886
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1888 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.490' WHERE PROPTB_TUSS_ID = 1887
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1889 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.533' WHERE PROPTB_TUSS_ID = 1888
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1890 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.573' WHERE PROPTB_TUSS_ID = 1889
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1891 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.617' WHERE PROPTB_TUSS_ID = 1890
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1892 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.663' WHERE PROPTB_TUSS_ID = 1891
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1893 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.703' WHERE PROPTB_TUSS_ID = 1892
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1894 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.747' WHERE PROPTB_TUSS_ID = 1893
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1895 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.787' WHERE PROPTB_TUSS_ID = 1894
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1896 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.833' WHERE PROPTB_TUSS_ID = 1895
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1897 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.873' WHERE PROPTB_TUSS_ID = 1896
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1898 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.917' WHERE PROPTB_TUSS_ID = 1897
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1899 , CLOUD_SYNC_DATE = '2014-05-06 17:36:07.957' WHERE PROPTB_TUSS_ID = 1898
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1900 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.000' WHERE PROPTB_TUSS_ID = 1899
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1901 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.043' WHERE PROPTB_TUSS_ID = 1900
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1902 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.083' WHERE PROPTB_TUSS_ID = 1901
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1903 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.130' WHERE PROPTB_TUSS_ID = 1902
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1904 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.177' WHERE PROPTB_TUSS_ID = 1903
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1905 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.220' WHERE PROPTB_TUSS_ID = 1904
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1906 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.263' WHERE PROPTB_TUSS_ID = 1905
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1907 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.303' WHERE PROPTB_TUSS_ID = 1906
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1908 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.350' WHERE PROPTB_TUSS_ID = 1907
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1909 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.390' WHERE PROPTB_TUSS_ID = 1908
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1910 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.433' WHERE PROPTB_TUSS_ID = 1909
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1911 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.473' WHERE PROPTB_TUSS_ID = 1910
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1912 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.517' WHERE PROPTB_TUSS_ID = 1911
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1913 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.557' WHERE PROPTB_TUSS_ID = 1912
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1914 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.603' WHERE PROPTB_TUSS_ID = 1913
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1915 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.647' WHERE PROPTB_TUSS_ID = 1914
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1916 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.687' WHERE PROPTB_TUSS_ID = 1915
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1917 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.730' WHERE PROPTB_TUSS_ID = 1916
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1918 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.773' WHERE PROPTB_TUSS_ID = 1917
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1919 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.817' WHERE PROPTB_TUSS_ID = 1918
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1920 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.857' WHERE PROPTB_TUSS_ID = 1919
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1921 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.897' WHERE PROPTB_TUSS_ID = 1920
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1922 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.940' WHERE PROPTB_TUSS_ID = 1921
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1923 , CLOUD_SYNC_DATE = '2014-05-06 17:36:08.980' WHERE PROPTB_TUSS_ID = 1922
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1924 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.023' WHERE PROPTB_TUSS_ID = 1923
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1925 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.067' WHERE PROPTB_TUSS_ID = 1924
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1926 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.110' WHERE PROPTB_TUSS_ID = 1925
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1927 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.157' WHERE PROPTB_TUSS_ID = 1926
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1928 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.200' WHERE PROPTB_TUSS_ID = 1927
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1929 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.243' WHERE PROPTB_TUSS_ID = 1928
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1930 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.290' WHERE PROPTB_TUSS_ID = 1929
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1931 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.333' WHERE PROPTB_TUSS_ID = 1930
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1932 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.373' WHERE PROPTB_TUSS_ID = 1931
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1933 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.420' WHERE PROPTB_TUSS_ID = 1932
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1934 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.460' WHERE PROPTB_TUSS_ID = 1933
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1935 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.500' WHERE PROPTB_TUSS_ID = 1934
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1936 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.547' WHERE PROPTB_TUSS_ID = 1935
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1937 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.590' WHERE PROPTB_TUSS_ID = 1936
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1938 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.633' WHERE PROPTB_TUSS_ID = 1937
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1939 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.680' WHERE PROPTB_TUSS_ID = 1938
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1940 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.720' WHERE PROPTB_TUSS_ID = 1939
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1941 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.760' WHERE PROPTB_TUSS_ID = 1940
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1942 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.800' WHERE PROPTB_TUSS_ID = 1941
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1943 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.847' WHERE PROPTB_TUSS_ID = 1942
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1944 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.893' WHERE PROPTB_TUSS_ID = 1943
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1945 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.937' WHERE PROPTB_TUSS_ID = 1944
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1946 , CLOUD_SYNC_DATE = '2014-05-06 17:36:09.980' WHERE PROPTB_TUSS_ID = 1945
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1947 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.020' WHERE PROPTB_TUSS_ID = 1946
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1948 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.063' WHERE PROPTB_TUSS_ID = 1947
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1949 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.103' WHERE PROPTB_TUSS_ID = 1948
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1950 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.147' WHERE PROPTB_TUSS_ID = 1949
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1951 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.187' WHERE PROPTB_TUSS_ID = 1950
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1952 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.230' WHERE PROPTB_TUSS_ID = 1951
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1953 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.270' WHERE PROPTB_TUSS_ID = 1952
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1954 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.313' WHERE PROPTB_TUSS_ID = 1953
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1955 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.353' WHERE PROPTB_TUSS_ID = 1954
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1956 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.397' WHERE PROPTB_TUSS_ID = 1955
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1957 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.437' WHERE PROPTB_TUSS_ID = 1956
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1958 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.477' WHERE PROPTB_TUSS_ID = 1957
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1959 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.520' WHERE PROPTB_TUSS_ID = 1958
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1960 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.567' WHERE PROPTB_TUSS_ID = 1959
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1961 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.610' WHERE PROPTB_TUSS_ID = 1960
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1962 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.657' WHERE PROPTB_TUSS_ID = 1961
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1963 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.697' WHERE PROPTB_TUSS_ID = 1962
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1964 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.740' WHERE PROPTB_TUSS_ID = 1963
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1965 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.783' WHERE PROPTB_TUSS_ID = 1964
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1966 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.823' WHERE PROPTB_TUSS_ID = 1965
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1967 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.867' WHERE PROPTB_TUSS_ID = 1966
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1968 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.907' WHERE PROPTB_TUSS_ID = 1967
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1969 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.947' WHERE PROPTB_TUSS_ID = 1968
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1970 , CLOUD_SYNC_DATE = '2014-05-06 17:36:10.993' WHERE PROPTB_TUSS_ID = 1969
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1971 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.043' WHERE PROPTB_TUSS_ID = 1970
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1972 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.090' WHERE PROPTB_TUSS_ID = 1971
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1973 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.137' WHERE PROPTB_TUSS_ID = 1972
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1974 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.180' WHERE PROPTB_TUSS_ID = 1973
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1975 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.223' WHERE PROPTB_TUSS_ID = 1974
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1976 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.267' WHERE PROPTB_TUSS_ID = 1975
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1977 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.310' WHERE PROPTB_TUSS_ID = 1976
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1978 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.350' WHERE PROPTB_TUSS_ID = 1977
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1979 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.393' WHERE PROPTB_TUSS_ID = 1978
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1980 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.437' WHERE PROPTB_TUSS_ID = 1979
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1981 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.480' WHERE PROPTB_TUSS_ID = 1980
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1982 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.523' WHERE PROPTB_TUSS_ID = 1981
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1983 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.577' WHERE PROPTB_TUSS_ID = 1982
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1984 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.623' WHERE PROPTB_TUSS_ID = 1983
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1985 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.667' WHERE PROPTB_TUSS_ID = 1984
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1986 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.710' WHERE PROPTB_TUSS_ID = 1985
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1987 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.750' WHERE PROPTB_TUSS_ID = 1986
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1988 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.790' WHERE PROPTB_TUSS_ID = 1987
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1989 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.833' WHERE PROPTB_TUSS_ID = 1988
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1990 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.873' WHERE PROPTB_TUSS_ID = 1989
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1991 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.917' WHERE PROPTB_TUSS_ID = 1990
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1992 , CLOUD_SYNC_DATE = '2014-05-06 17:36:11.963' WHERE PROPTB_TUSS_ID = 1991
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1993 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.007' WHERE PROPTB_TUSS_ID = 1992
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1994 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.047' WHERE PROPTB_TUSS_ID = 1993
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1995 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.090' WHERE PROPTB_TUSS_ID = 1994
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1996 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.133' WHERE PROPTB_TUSS_ID = 1995
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1997 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.173' WHERE PROPTB_TUSS_ID = 1996
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1998 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.220' WHERE PROPTB_TUSS_ID = 1997
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 1999 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.263' WHERE PROPTB_TUSS_ID = 1998
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2000 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.317' WHERE PROPTB_TUSS_ID = 1999
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2001 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.360' WHERE PROPTB_TUSS_ID = 2000
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2002 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.403' WHERE PROPTB_TUSS_ID = 2001
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2003 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.450' WHERE PROPTB_TUSS_ID = 2002
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2004 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.493' WHERE PROPTB_TUSS_ID = 2003
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2005 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.540' WHERE PROPTB_TUSS_ID = 2004
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2006 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.583' WHERE PROPTB_TUSS_ID = 2005
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2007 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.627' WHERE PROPTB_TUSS_ID = 2006
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2008 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.670' WHERE PROPTB_TUSS_ID = 2007
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2009 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.713' WHERE PROPTB_TUSS_ID = 2008
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2010 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.757' WHERE PROPTB_TUSS_ID = 2009
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2011 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.800' WHERE PROPTB_TUSS_ID = 2010
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2012 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.843' WHERE PROPTB_TUSS_ID = 2011
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2013 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.887' WHERE PROPTB_TUSS_ID = 2012
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2014 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.927' WHERE PROPTB_TUSS_ID = 2013
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2015 , CLOUD_SYNC_DATE = '2014-05-06 17:36:12.970' WHERE PROPTB_TUSS_ID = 2014
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2016 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.013' WHERE PROPTB_TUSS_ID = 2015
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2017 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.060' WHERE PROPTB_TUSS_ID = 2016
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2018 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.103' WHERE PROPTB_TUSS_ID = 2017
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2019 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.147' WHERE PROPTB_TUSS_ID = 2018
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2020 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.190' WHERE PROPTB_TUSS_ID = 2019
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2021 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.233' WHERE PROPTB_TUSS_ID = 2020
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2022 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.280' WHERE PROPTB_TUSS_ID = 2021
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2023 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.327' WHERE PROPTB_TUSS_ID = 2022
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2024 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.373' WHERE PROPTB_TUSS_ID = 2023
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2025 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.417' WHERE PROPTB_TUSS_ID = 2024
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2026 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.457' WHERE PROPTB_TUSS_ID = 2025
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2027 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.503' WHERE PROPTB_TUSS_ID = 2026
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2028 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.550' WHERE PROPTB_TUSS_ID = 2027
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2029 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.597' WHERE PROPTB_TUSS_ID = 2028
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2030 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.640' WHERE PROPTB_TUSS_ID = 2029
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2031 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.687' WHERE PROPTB_TUSS_ID = 2030
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2032 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.730' WHERE PROPTB_TUSS_ID = 2031
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2033 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.780' WHERE PROPTB_TUSS_ID = 2032
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2034 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.823' WHERE PROPTB_TUSS_ID = 2033
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2035 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.867' WHERE PROPTB_TUSS_ID = 2034
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2036 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.910' WHERE PROPTB_TUSS_ID = 2035
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2037 , CLOUD_SYNC_DATE = '2014-05-06 17:36:13.957' WHERE PROPTB_TUSS_ID = 2036
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2038 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.000' WHERE PROPTB_TUSS_ID = 2037
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2039 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.043' WHERE PROPTB_TUSS_ID = 2038
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2040 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.090' WHERE PROPTB_TUSS_ID = 2039
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2041 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.137' WHERE PROPTB_TUSS_ID = 2040
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2042 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.177' WHERE PROPTB_TUSS_ID = 2041
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2043 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.220' WHERE PROPTB_TUSS_ID = 2042
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2044 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.267' WHERE PROPTB_TUSS_ID = 2043
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2045 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.310' WHERE PROPTB_TUSS_ID = 2044
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2046 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.357' WHERE PROPTB_TUSS_ID = 2045
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2047 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.400' WHERE PROPTB_TUSS_ID = 2046
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2048 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.447' WHERE PROPTB_TUSS_ID = 2047
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2049 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.490' WHERE PROPTB_TUSS_ID = 2048
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2050 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.533' WHERE PROPTB_TUSS_ID = 2049
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2051 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.577' WHERE PROPTB_TUSS_ID = 2050
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2052 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.623' WHERE PROPTB_TUSS_ID = 2051
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2053 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.673' WHERE PROPTB_TUSS_ID = 2052
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2054 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.717' WHERE PROPTB_TUSS_ID = 2053
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2055 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.763' WHERE PROPTB_TUSS_ID = 2054
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2056 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.807' WHERE PROPTB_TUSS_ID = 2055
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2057 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.850' WHERE PROPTB_TUSS_ID = 2056
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2058 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.900' WHERE PROPTB_TUSS_ID = 2057
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2059 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.943' WHERE PROPTB_TUSS_ID = 2058
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2060 , CLOUD_SYNC_DATE = '2014-05-06 17:36:14.987' WHERE PROPTB_TUSS_ID = 2059
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2061 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.030' WHERE PROPTB_TUSS_ID = 2060
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2062 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.077' WHERE PROPTB_TUSS_ID = 2061
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2063 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.120' WHERE PROPTB_TUSS_ID = 2062
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2064 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.163' WHERE PROPTB_TUSS_ID = 2063
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2065 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.207' WHERE PROPTB_TUSS_ID = 2064
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2066 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.250' WHERE PROPTB_TUSS_ID = 2065
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2067 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.297' WHERE PROPTB_TUSS_ID = 2066
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2068 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.337' WHERE PROPTB_TUSS_ID = 2067
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2069 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.383' WHERE PROPTB_TUSS_ID = 2068
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2070 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.427' WHERE PROPTB_TUSS_ID = 2069
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2071 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.473' WHERE PROPTB_TUSS_ID = 2070
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2072 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.517' WHERE PROPTB_TUSS_ID = 2071
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2073 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.560' WHERE PROPTB_TUSS_ID = 2072
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2074 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.603' WHERE PROPTB_TUSS_ID = 2073
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2075 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.650' WHERE PROPTB_TUSS_ID = 2074
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2076 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.693' WHERE PROPTB_TUSS_ID = 2075
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2077 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.737' WHERE PROPTB_TUSS_ID = 2076
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2078 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.783' WHERE PROPTB_TUSS_ID = 2077
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2079 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.827' WHERE PROPTB_TUSS_ID = 2078
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2080 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.870' WHERE PROPTB_TUSS_ID = 2079
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2081 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.917' WHERE PROPTB_TUSS_ID = 2080
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2082 , CLOUD_SYNC_DATE = '2014-05-06 17:36:15.957' WHERE PROPTB_TUSS_ID = 2081
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2083 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.003' WHERE PROPTB_TUSS_ID = 2082
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2084 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.053' WHERE PROPTB_TUSS_ID = 2083
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2085 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.097' WHERE PROPTB_TUSS_ID = 2084
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2086 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.140' WHERE PROPTB_TUSS_ID = 2085
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2087 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.187' WHERE PROPTB_TUSS_ID = 2086
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2088 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.237' WHERE PROPTB_TUSS_ID = 2087
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2089 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.283' WHERE PROPTB_TUSS_ID = 2088
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2090 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.327' WHERE PROPTB_TUSS_ID = 2089
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2091 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.373' WHERE PROPTB_TUSS_ID = 2090
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2092 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.420' WHERE PROPTB_TUSS_ID = 2091
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2093 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.463' WHERE PROPTB_TUSS_ID = 2092
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2094 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.507' WHERE PROPTB_TUSS_ID = 2093
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2095 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.550' WHERE PROPTB_TUSS_ID = 2094
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2096 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.600' WHERE PROPTB_TUSS_ID = 2095
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2097 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.647' WHERE PROPTB_TUSS_ID = 2096
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2098 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.693' WHERE PROPTB_TUSS_ID = 2097
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2099 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.743' WHERE PROPTB_TUSS_ID = 2098
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2100 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.790' WHERE PROPTB_TUSS_ID = 2099
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2101 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.837' WHERE PROPTB_TUSS_ID = 2100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2102 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.883' WHERE PROPTB_TUSS_ID = 2101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2103 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.930' WHERE PROPTB_TUSS_ID = 2102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2104 , CLOUD_SYNC_DATE = '2014-05-06 17:36:16.977' WHERE PROPTB_TUSS_ID = 2103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2105 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.027' WHERE PROPTB_TUSS_ID = 2104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2106 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.073' WHERE PROPTB_TUSS_ID = 2105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2107 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.127' WHERE PROPTB_TUSS_ID = 2106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2108 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.170' WHERE PROPTB_TUSS_ID = 2107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2109 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.217' WHERE PROPTB_TUSS_ID = 2108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2110 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.263' WHERE PROPTB_TUSS_ID = 2109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2111 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.310' WHERE PROPTB_TUSS_ID = 2110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2112 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.357' WHERE PROPTB_TUSS_ID = 2111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2113 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.407' WHERE PROPTB_TUSS_ID = 2112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2114 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.453' WHERE PROPTB_TUSS_ID = 2113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2115 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.497' WHERE PROPTB_TUSS_ID = 2114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2116 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.543' WHERE PROPTB_TUSS_ID = 2115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2117 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.593' WHERE PROPTB_TUSS_ID = 2116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2118 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.650' WHERE PROPTB_TUSS_ID = 2117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2119 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.707' WHERE PROPTB_TUSS_ID = 2118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2120 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.750' WHERE PROPTB_TUSS_ID = 2119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2121 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.807' WHERE PROPTB_TUSS_ID = 2120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2122 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.850' WHERE PROPTB_TUSS_ID = 2121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2123 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.897' WHERE PROPTB_TUSS_ID = 2122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2124 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.947' WHERE PROPTB_TUSS_ID = 2123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2125 , CLOUD_SYNC_DATE = '2014-05-06 17:36:17.990' WHERE PROPTB_TUSS_ID = 2124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2126 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.037' WHERE PROPTB_TUSS_ID = 2125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2127 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.087' WHERE PROPTB_TUSS_ID = 2126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2128 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.137' WHERE PROPTB_TUSS_ID = 2127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2129 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.180' WHERE PROPTB_TUSS_ID = 2128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2130 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.227' WHERE PROPTB_TUSS_ID = 2129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2131 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.277' WHERE PROPTB_TUSS_ID = 2130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2132 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.323' WHERE PROPTB_TUSS_ID = 2131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2133 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.367' WHERE PROPTB_TUSS_ID = 2132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2134 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.413' WHERE PROPTB_TUSS_ID = 2133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2135 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.467' WHERE PROPTB_TUSS_ID = 2134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2136 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.517' WHERE PROPTB_TUSS_ID = 2135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2137 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.567' WHERE PROPTB_TUSS_ID = 2136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2138 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.617' WHERE PROPTB_TUSS_ID = 2137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2139 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.667' WHERE PROPTB_TUSS_ID = 2138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2140 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.710' WHERE PROPTB_TUSS_ID = 2139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2141 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.760' WHERE PROPTB_TUSS_ID = 2140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2142 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.803' WHERE PROPTB_TUSS_ID = 2141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2143 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.847' WHERE PROPTB_TUSS_ID = 2142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2144 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.897' WHERE PROPTB_TUSS_ID = 2143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2145 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.943' WHERE PROPTB_TUSS_ID = 2144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2146 , CLOUD_SYNC_DATE = '2014-05-06 17:36:18.987' WHERE PROPTB_TUSS_ID = 2145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2147 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.037' WHERE PROPTB_TUSS_ID = 2146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2148 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.080' WHERE PROPTB_TUSS_ID = 2147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2149 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.127' WHERE PROPTB_TUSS_ID = 2148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2150 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.170' WHERE PROPTB_TUSS_ID = 2149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2151 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.217' WHERE PROPTB_TUSS_ID = 2150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2152 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.263' WHERE PROPTB_TUSS_ID = 2151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2153 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.307' WHERE PROPTB_TUSS_ID = 2152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2154 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.357' WHERE PROPTB_TUSS_ID = 2153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2155 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.400' WHERE PROPTB_TUSS_ID = 2154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2156 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.447' WHERE PROPTB_TUSS_ID = 2155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2157 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.497' WHERE PROPTB_TUSS_ID = 2156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2158 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.540' WHERE PROPTB_TUSS_ID = 2157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2159 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.587' WHERE PROPTB_TUSS_ID = 2158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2160 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.633' WHERE PROPTB_TUSS_ID = 2159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2161 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.683' WHERE PROPTB_TUSS_ID = 2160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2162 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.727' WHERE PROPTB_TUSS_ID = 2161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2163 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.777' WHERE PROPTB_TUSS_ID = 2162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2164 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.833' WHERE PROPTB_TUSS_ID = 2163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2165 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.887' WHERE PROPTB_TUSS_ID = 2164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2166 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.937' WHERE PROPTB_TUSS_ID = 2165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2167 , CLOUD_SYNC_DATE = '2014-05-06 17:36:19.983' WHERE PROPTB_TUSS_ID = 2166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2168 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.030' WHERE PROPTB_TUSS_ID = 2167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2169 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.087' WHERE PROPTB_TUSS_ID = 2168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2170 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.140' WHERE PROPTB_TUSS_ID = 2169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2171 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.187' WHERE PROPTB_TUSS_ID = 2170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2172 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.237' WHERE PROPTB_TUSS_ID = 2171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2173 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.280' WHERE PROPTB_TUSS_ID = 2172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2174 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.330' WHERE PROPTB_TUSS_ID = 2173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2175 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.377' WHERE PROPTB_TUSS_ID = 2174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2176 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.423' WHERE PROPTB_TUSS_ID = 2175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2177 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.470' WHERE PROPTB_TUSS_ID = 2176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2178 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.517' WHERE PROPTB_TUSS_ID = 2177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2179 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.567' WHERE PROPTB_TUSS_ID = 2178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2180 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.617' WHERE PROPTB_TUSS_ID = 2179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2181 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.670' WHERE PROPTB_TUSS_ID = 2180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2182 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.717' WHERE PROPTB_TUSS_ID = 2181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2183 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.767' WHERE PROPTB_TUSS_ID = 2182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2184 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.813' WHERE PROPTB_TUSS_ID = 2183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2185 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.863' WHERE PROPTB_TUSS_ID = 2184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2186 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.913' WHERE PROPTB_TUSS_ID = 2185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2187 , CLOUD_SYNC_DATE = '2014-05-06 17:36:20.963' WHERE PROPTB_TUSS_ID = 2186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2188 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.013' WHERE PROPTB_TUSS_ID = 2187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2189 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.060' WHERE PROPTB_TUSS_ID = 2188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2190 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.107' WHERE PROPTB_TUSS_ID = 2189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2191 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.157' WHERE PROPTB_TUSS_ID = 2190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2192 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.203' WHERE PROPTB_TUSS_ID = 2191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2193 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.257' WHERE PROPTB_TUSS_ID = 2192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2194 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.310' WHERE PROPTB_TUSS_ID = 2193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2195 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.363' WHERE PROPTB_TUSS_ID = 2194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2196 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.413' WHERE PROPTB_TUSS_ID = 2195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2197 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.463' WHERE PROPTB_TUSS_ID = 2196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2198 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.510' WHERE PROPTB_TUSS_ID = 2197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2199 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.560' WHERE PROPTB_TUSS_ID = 2198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2200 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.610' WHERE PROPTB_TUSS_ID = 2199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2201 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.677' WHERE PROPTB_TUSS_ID = 2200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2202 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.733' WHERE PROPTB_TUSS_ID = 2201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2203 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.780' WHERE PROPTB_TUSS_ID = 2202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2204 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.827' WHERE PROPTB_TUSS_ID = 2203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2205 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.880' WHERE PROPTB_TUSS_ID = 2204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2206 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.930' WHERE PROPTB_TUSS_ID = 2205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2207 , CLOUD_SYNC_DATE = '2014-05-06 17:36:21.980' WHERE PROPTB_TUSS_ID = 2206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2208 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.027' WHERE PROPTB_TUSS_ID = 2207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2209 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.073' WHERE PROPTB_TUSS_ID = 2208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2210 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.127' WHERE PROPTB_TUSS_ID = 2209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2211 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.173' WHERE PROPTB_TUSS_ID = 2210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2212 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.220' WHERE PROPTB_TUSS_ID = 2211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2213 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.273' WHERE PROPTB_TUSS_ID = 2212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2214 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.323' WHERE PROPTB_TUSS_ID = 2213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2215 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.377' WHERE PROPTB_TUSS_ID = 2214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2216 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.420' WHERE PROPTB_TUSS_ID = 2215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2217 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.467' WHERE PROPTB_TUSS_ID = 2216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2218 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.517' WHERE PROPTB_TUSS_ID = 2217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2219 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.560' WHERE PROPTB_TUSS_ID = 2218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2220 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.610' WHERE PROPTB_TUSS_ID = 2219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2221 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.660' WHERE PROPTB_TUSS_ID = 2220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2222 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.707' WHERE PROPTB_TUSS_ID = 2221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2223 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.757' WHERE PROPTB_TUSS_ID = 2222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2224 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.807' WHERE PROPTB_TUSS_ID = 2223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2225 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.853' WHERE PROPTB_TUSS_ID = 2224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2226 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.903' WHERE PROPTB_TUSS_ID = 2225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2227 , CLOUD_SYNC_DATE = '2014-05-06 17:36:22.970' WHERE PROPTB_TUSS_ID = 2226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2228 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.023' WHERE PROPTB_TUSS_ID = 2227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2229 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.077' WHERE PROPTB_TUSS_ID = 2228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2230 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.123' WHERE PROPTB_TUSS_ID = 2229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2231 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.177' WHERE PROPTB_TUSS_ID = 2230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2232 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.227' WHERE PROPTB_TUSS_ID = 2231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2233 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.277' WHERE PROPTB_TUSS_ID = 2232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2234 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.327' WHERE PROPTB_TUSS_ID = 2233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2235 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.380' WHERE PROPTB_TUSS_ID = 2234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2236 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.427' WHERE PROPTB_TUSS_ID = 2235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2237 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.477' WHERE PROPTB_TUSS_ID = 2236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2238 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.530' WHERE PROPTB_TUSS_ID = 2237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2239 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.577' WHERE PROPTB_TUSS_ID = 2238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2240 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.627' WHERE PROPTB_TUSS_ID = 2239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2241 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.680' WHERE PROPTB_TUSS_ID = 2240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2242 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.727' WHERE PROPTB_TUSS_ID = 2241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2243 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.777' WHERE PROPTB_TUSS_ID = 2242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2244 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.823' WHERE PROPTB_TUSS_ID = 2243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2245 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.873' WHERE PROPTB_TUSS_ID = 2244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2246 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.923' WHERE PROPTB_TUSS_ID = 2245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2247 , CLOUD_SYNC_DATE = '2014-05-06 17:36:23.987' WHERE PROPTB_TUSS_ID = 2246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2248 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.033' WHERE PROPTB_TUSS_ID = 2247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2249 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.083' WHERE PROPTB_TUSS_ID = 2248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2250 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.133' WHERE PROPTB_TUSS_ID = 2249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2251 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.180' WHERE PROPTB_TUSS_ID = 2250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2252 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.240' WHERE PROPTB_TUSS_ID = 2251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2253 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.297' WHERE PROPTB_TUSS_ID = 2252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2254 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.347' WHERE PROPTB_TUSS_ID = 2253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2255 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.393' WHERE PROPTB_TUSS_ID = 2254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2256 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.440' WHERE PROPTB_TUSS_ID = 2255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2257 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.490' WHERE PROPTB_TUSS_ID = 2256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2258 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.540' WHERE PROPTB_TUSS_ID = 2257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2259 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.590' WHERE PROPTB_TUSS_ID = 2258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2260 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.640' WHERE PROPTB_TUSS_ID = 2259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2261 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.700' WHERE PROPTB_TUSS_ID = 2260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2262 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.747' WHERE PROPTB_TUSS_ID = 2261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2263 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.800' WHERE PROPTB_TUSS_ID = 2262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2264 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.857' WHERE PROPTB_TUSS_ID = 2263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2265 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.913' WHERE PROPTB_TUSS_ID = 2264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2266 , CLOUD_SYNC_DATE = '2014-05-06 17:36:24.963' WHERE PROPTB_TUSS_ID = 2265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2267 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.010' WHERE PROPTB_TUSS_ID = 2266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2268 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.063' WHERE PROPTB_TUSS_ID = 2267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2269 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.110' WHERE PROPTB_TUSS_ID = 2268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2270 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.160' WHERE PROPTB_TUSS_ID = 2269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2271 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.207' WHERE PROPTB_TUSS_ID = 2270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2272 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.257' WHERE PROPTB_TUSS_ID = 2271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2273 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.307' WHERE PROPTB_TUSS_ID = 2272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2274 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.353' WHERE PROPTB_TUSS_ID = 2273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2275 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.403' WHERE PROPTB_TUSS_ID = 2274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2276 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.450' WHERE PROPTB_TUSS_ID = 2275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2277 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.500' WHERE PROPTB_TUSS_ID = 2276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2278 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.547' WHERE PROPTB_TUSS_ID = 2277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2279 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.600' WHERE PROPTB_TUSS_ID = 2278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2280 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.650' WHERE PROPTB_TUSS_ID = 2279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2281 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.700' WHERE PROPTB_TUSS_ID = 2280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2282 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.750' WHERE PROPTB_TUSS_ID = 2281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2283 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.813' WHERE PROPTB_TUSS_ID = 2282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2284 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.863' WHERE PROPTB_TUSS_ID = 2283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2285 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.917' WHERE PROPTB_TUSS_ID = 2284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2286 , CLOUD_SYNC_DATE = '2014-05-06 17:36:25.967' WHERE PROPTB_TUSS_ID = 2285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2287 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.017' WHERE PROPTB_TUSS_ID = 2286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2288 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.067' WHERE PROPTB_TUSS_ID = 2287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2289 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.117' WHERE PROPTB_TUSS_ID = 2288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2290 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.167' WHERE PROPTB_TUSS_ID = 2289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2291 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.217' WHERE PROPTB_TUSS_ID = 2290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2292 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.267' WHERE PROPTB_TUSS_ID = 2291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2293 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.313' WHERE PROPTB_TUSS_ID = 2292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2294 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.367' WHERE PROPTB_TUSS_ID = 2293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2295 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.413' WHERE PROPTB_TUSS_ID = 2294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2296 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.467' WHERE PROPTB_TUSS_ID = 2295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2297 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.513' WHERE PROPTB_TUSS_ID = 2296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2298 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.567' WHERE PROPTB_TUSS_ID = 2297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2299 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.617' WHERE PROPTB_TUSS_ID = 2298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2300 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.677' WHERE PROPTB_TUSS_ID = 2299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2301 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.727' WHERE PROPTB_TUSS_ID = 2300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2302 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.777' WHERE PROPTB_TUSS_ID = 2301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2303 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.827' WHERE PROPTB_TUSS_ID = 2302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2304 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.877' WHERE PROPTB_TUSS_ID = 2303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2305 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.927' WHERE PROPTB_TUSS_ID = 2304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2306 , CLOUD_SYNC_DATE = '2014-05-06 17:36:26.977' WHERE PROPTB_TUSS_ID = 2305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2307 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.043' WHERE PROPTB_TUSS_ID = 2306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2308 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.100' WHERE PROPTB_TUSS_ID = 2307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2309 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.150' WHERE PROPTB_TUSS_ID = 2308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2310 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.200' WHERE PROPTB_TUSS_ID = 2309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2311 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.250' WHERE PROPTB_TUSS_ID = 2310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2312 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.300' WHERE PROPTB_TUSS_ID = 2311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2313 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.353' WHERE PROPTB_TUSS_ID = 2312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2314 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.400' WHERE PROPTB_TUSS_ID = 2313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2315 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.453' WHERE PROPTB_TUSS_ID = 2314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2316 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.500' WHERE PROPTB_TUSS_ID = 2315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2317 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.553' WHERE PROPTB_TUSS_ID = 2316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2318 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.603' WHERE PROPTB_TUSS_ID = 2317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2319 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.660' WHERE PROPTB_TUSS_ID = 2318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2320 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.713' WHERE PROPTB_TUSS_ID = 2319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2321 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.767' WHERE PROPTB_TUSS_ID = 2320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2322 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.813' WHERE PROPTB_TUSS_ID = 2321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2323 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.870' WHERE PROPTB_TUSS_ID = 2322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2324 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.917' WHERE PROPTB_TUSS_ID = 2323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2325 , CLOUD_SYNC_DATE = '2014-05-06 17:36:27.970' WHERE PROPTB_TUSS_ID = 2324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2326 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.020' WHERE PROPTB_TUSS_ID = 2325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2327 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.073' WHERE PROPTB_TUSS_ID = 2326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2328 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.123' WHERE PROPTB_TUSS_ID = 2327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2329 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.173' WHERE PROPTB_TUSS_ID = 2328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2330 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.223' WHERE PROPTB_TUSS_ID = 2329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2331 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.277' WHERE PROPTB_TUSS_ID = 2330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2332 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.323' WHERE PROPTB_TUSS_ID = 2331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2333 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.373' WHERE PROPTB_TUSS_ID = 2332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2334 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.423' WHERE PROPTB_TUSS_ID = 2333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2335 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.473' WHERE PROPTB_TUSS_ID = 2334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2336 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.523' WHERE PROPTB_TUSS_ID = 2335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2337 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.573' WHERE PROPTB_TUSS_ID = 2336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2338 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.623' WHERE PROPTB_TUSS_ID = 2337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2339 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.680' WHERE PROPTB_TUSS_ID = 2338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2340 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.733' WHERE PROPTB_TUSS_ID = 2339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2341 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.780' WHERE PROPTB_TUSS_ID = 2340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2342 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.833' WHERE PROPTB_TUSS_ID = 2341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2343 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.900' WHERE PROPTB_TUSS_ID = 2342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2344 , CLOUD_SYNC_DATE = '2014-05-06 17:36:28.953' WHERE PROPTB_TUSS_ID = 2343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2345 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.003' WHERE PROPTB_TUSS_ID = 2344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2346 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.053' WHERE PROPTB_TUSS_ID = 2345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2347 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.103' WHERE PROPTB_TUSS_ID = 2346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2348 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.153' WHERE PROPTB_TUSS_ID = 2347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2349 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.203' WHERE PROPTB_TUSS_ID = 2348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2350 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.253' WHERE PROPTB_TUSS_ID = 2349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2351 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.303' WHERE PROPTB_TUSS_ID = 2350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2352 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.353' WHERE PROPTB_TUSS_ID = 2351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2353 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.403' WHERE PROPTB_TUSS_ID = 2352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2354 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.453' WHERE PROPTB_TUSS_ID = 2353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2355 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.503' WHERE PROPTB_TUSS_ID = 2354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2356 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.557' WHERE PROPTB_TUSS_ID = 2355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2357 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.610' WHERE PROPTB_TUSS_ID = 2356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2358 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.663' WHERE PROPTB_TUSS_ID = 2357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2359 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.717' WHERE PROPTB_TUSS_ID = 2358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2360 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.767' WHERE PROPTB_TUSS_ID = 2359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2361 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.817' WHERE PROPTB_TUSS_ID = 2360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2362 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.867' WHERE PROPTB_TUSS_ID = 2361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2363 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.917' WHERE PROPTB_TUSS_ID = 2362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2364 , CLOUD_SYNC_DATE = '2014-05-06 17:36:29.967' WHERE PROPTB_TUSS_ID = 2363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2365 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.017' WHERE PROPTB_TUSS_ID = 2364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2366 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.070' WHERE PROPTB_TUSS_ID = 2365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2367 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.120' WHERE PROPTB_TUSS_ID = 2366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2368 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.170' WHERE PROPTB_TUSS_ID = 2367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2369 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.220' WHERE PROPTB_TUSS_ID = 2368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2370 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.273' WHERE PROPTB_TUSS_ID = 2369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2371 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.333' WHERE PROPTB_TUSS_ID = 2370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2372 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.390' WHERE PROPTB_TUSS_ID = 2371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2373 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.443' WHERE PROPTB_TUSS_ID = 2372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2374 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.493' WHERE PROPTB_TUSS_ID = 2373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2375 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.547' WHERE PROPTB_TUSS_ID = 2374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2376 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.603' WHERE PROPTB_TUSS_ID = 2375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2377 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.673' WHERE PROPTB_TUSS_ID = 2376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2378 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.737' WHERE PROPTB_TUSS_ID = 2377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2379 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.800' WHERE PROPTB_TUSS_ID = 2378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2380 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.873' WHERE PROPTB_TUSS_ID = 2379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2381 , CLOUD_SYNC_DATE = '2014-05-06 17:36:30.957' WHERE PROPTB_TUSS_ID = 2380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2382 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.013' WHERE PROPTB_TUSS_ID = 2381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2383 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.070' WHERE PROPTB_TUSS_ID = 2382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2384 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.123' WHERE PROPTB_TUSS_ID = 2383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2385 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.173' WHERE PROPTB_TUSS_ID = 2384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2386 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.227' WHERE PROPTB_TUSS_ID = 2385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2387 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.277' WHERE PROPTB_TUSS_ID = 2386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2388 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.333' WHERE PROPTB_TUSS_ID = 2387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2389 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.387' WHERE PROPTB_TUSS_ID = 2388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2390 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.437' WHERE PROPTB_TUSS_ID = 2389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2391 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.490' WHERE PROPTB_TUSS_ID = 2390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2392 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.553' WHERE PROPTB_TUSS_ID = 2391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2393 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.620' WHERE PROPTB_TUSS_ID = 2392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2394 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.673' WHERE PROPTB_TUSS_ID = 2393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2395 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.733' WHERE PROPTB_TUSS_ID = 2394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2396 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.793' WHERE PROPTB_TUSS_ID = 2395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2397 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.800' WHERE PROPTB_TUSS_ID = 2396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2398 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.897' WHERE PROPTB_TUSS_ID = 2397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2399 , CLOUD_SYNC_DATE = '2014-05-06 17:36:31.950' WHERE PROPTB_TUSS_ID = 2398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2400 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.003' WHERE PROPTB_TUSS_ID = 2399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2401 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.057' WHERE PROPTB_TUSS_ID = 2400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2402 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.107' WHERE PROPTB_TUSS_ID = 2401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2403 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.160' WHERE PROPTB_TUSS_ID = 2402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2404 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.210' WHERE PROPTB_TUSS_ID = 2403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2405 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.263' WHERE PROPTB_TUSS_ID = 2404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2406 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.310' WHERE PROPTB_TUSS_ID = 2405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2407 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.363' WHERE PROPTB_TUSS_ID = 2406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2408 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.417' WHERE PROPTB_TUSS_ID = 2407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2409 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.467' WHERE PROPTB_TUSS_ID = 2408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2410 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.520' WHERE PROPTB_TUSS_ID = 2409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2411 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.570' WHERE PROPTB_TUSS_ID = 2410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2412 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.627' WHERE PROPTB_TUSS_ID = 2411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2413 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.683' WHERE PROPTB_TUSS_ID = 2412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2414 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.733' WHERE PROPTB_TUSS_ID = 2413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2415 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.787' WHERE PROPTB_TUSS_ID = 2414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2416 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.840' WHERE PROPTB_TUSS_ID = 2415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2417 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.907' WHERE PROPTB_TUSS_ID = 2416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2418 , CLOUD_SYNC_DATE = '2014-05-06 17:36:32.970' WHERE PROPTB_TUSS_ID = 2417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2419 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.020' WHERE PROPTB_TUSS_ID = 2418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2420 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.073' WHERE PROPTB_TUSS_ID = 2419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2421 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.127' WHERE PROPTB_TUSS_ID = 2420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2422 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.183' WHERE PROPTB_TUSS_ID = 2421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2423 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.237' WHERE PROPTB_TUSS_ID = 2422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2424 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.290' WHERE PROPTB_TUSS_ID = 2423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2425 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.343' WHERE PROPTB_TUSS_ID = 2424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2426 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.397' WHERE PROPTB_TUSS_ID = 2425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2427 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.450' WHERE PROPTB_TUSS_ID = 2426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2428 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.503' WHERE PROPTB_TUSS_ID = 2427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2429 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.557' WHERE PROPTB_TUSS_ID = 2428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2430 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.607' WHERE PROPTB_TUSS_ID = 2429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2431 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.667' WHERE PROPTB_TUSS_ID = 2430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2432 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.720' WHERE PROPTB_TUSS_ID = 2431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2433 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.773' WHERE PROPTB_TUSS_ID = 2432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2434 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.827' WHERE PROPTB_TUSS_ID = 2433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2435 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.890' WHERE PROPTB_TUSS_ID = 2434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2436 , CLOUD_SYNC_DATE = '2014-05-06 17:36:33.953' WHERE PROPTB_TUSS_ID = 2435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2437 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.007' WHERE PROPTB_TUSS_ID = 2436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2438 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.060' WHERE PROPTB_TUSS_ID = 2437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2439 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.113' WHERE PROPTB_TUSS_ID = 2438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2440 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.167' WHERE PROPTB_TUSS_ID = 2439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2441 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.220' WHERE PROPTB_TUSS_ID = 2440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2442 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.273' WHERE PROPTB_TUSS_ID = 2441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2443 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.323' WHERE PROPTB_TUSS_ID = 2442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2444 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.377' WHERE PROPTB_TUSS_ID = 2443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2445 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.433' WHERE PROPTB_TUSS_ID = 2444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2446 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.483' WHERE PROPTB_TUSS_ID = 2445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2447 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.537' WHERE PROPTB_TUSS_ID = 2446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2448 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.587' WHERE PROPTB_TUSS_ID = 2447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2449 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.640' WHERE PROPTB_TUSS_ID = 2448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2450 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.703' WHERE PROPTB_TUSS_ID = 2449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2451 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.753' WHERE PROPTB_TUSS_ID = 2450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2452 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.807' WHERE PROPTB_TUSS_ID = 2451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2453 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.863' WHERE PROPTB_TUSS_ID = 2452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2454 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.917' WHERE PROPTB_TUSS_ID = 2453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2455 , CLOUD_SYNC_DATE = '2014-05-06 17:36:34.973' WHERE PROPTB_TUSS_ID = 2454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2456 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.027' WHERE PROPTB_TUSS_ID = 2455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2457 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.077' WHERE PROPTB_TUSS_ID = 2456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2458 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.133' WHERE PROPTB_TUSS_ID = 2457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2459 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.187' WHERE PROPTB_TUSS_ID = 2458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2460 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.237' WHERE PROPTB_TUSS_ID = 2459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2461 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.297' WHERE PROPTB_TUSS_ID = 2460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2462 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.363' WHERE PROPTB_TUSS_ID = 2461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2463 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.413' WHERE PROPTB_TUSS_ID = 2462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2464 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.470' WHERE PROPTB_TUSS_ID = 2463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2465 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.523' WHERE PROPTB_TUSS_ID = 2464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2466 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.580' WHERE PROPTB_TUSS_ID = 2465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2467 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.637' WHERE PROPTB_TUSS_ID = 2466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2468 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.710' WHERE PROPTB_TUSS_ID = 2467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2469 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.767' WHERE PROPTB_TUSS_ID = 2468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2470 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.827' WHERE PROPTB_TUSS_ID = 2469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2471 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.887' WHERE PROPTB_TUSS_ID = 2470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2472 , CLOUD_SYNC_DATE = '2014-05-06 17:36:35.947' WHERE PROPTB_TUSS_ID = 2471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2473 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.040' WHERE PROPTB_TUSS_ID = 2472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2474 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.093' WHERE PROPTB_TUSS_ID = 2473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2475 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.170' WHERE PROPTB_TUSS_ID = 2474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2476 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.250' WHERE PROPTB_TUSS_ID = 2475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2477 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.307' WHERE PROPTB_TUSS_ID = 2476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2478 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.367' WHERE PROPTB_TUSS_ID = 2477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2479 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.420' WHERE PROPTB_TUSS_ID = 2478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2480 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.477' WHERE PROPTB_TUSS_ID = 2479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2481 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.533' WHERE PROPTB_TUSS_ID = 2480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2482 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.593' WHERE PROPTB_TUSS_ID = 2481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2483 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.653' WHERE PROPTB_TUSS_ID = 2482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2484 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.713' WHERE PROPTB_TUSS_ID = 2483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2485 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.770' WHERE PROPTB_TUSS_ID = 2484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2486 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.827' WHERE PROPTB_TUSS_ID = 2485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2487 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.880' WHERE PROPTB_TUSS_ID = 2486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2488 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.937' WHERE PROPTB_TUSS_ID = 2487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2489 , CLOUD_SYNC_DATE = '2014-05-06 17:36:36.993' WHERE PROPTB_TUSS_ID = 2488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2490 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.050' WHERE PROPTB_TUSS_ID = 2489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2491 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.107' WHERE PROPTB_TUSS_ID = 2490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2492 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.167' WHERE PROPTB_TUSS_ID = 2491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2493 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.220' WHERE PROPTB_TUSS_ID = 2492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2494 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.283' WHERE PROPTB_TUSS_ID = 2493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2495 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.340' WHERE PROPTB_TUSS_ID = 2494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2496 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.397' WHERE PROPTB_TUSS_ID = 2495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2497 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.447' WHERE PROPTB_TUSS_ID = 2496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2498 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.503' WHERE PROPTB_TUSS_ID = 2497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2499 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.560' WHERE PROPTB_TUSS_ID = 2498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2500 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.613' WHERE PROPTB_TUSS_ID = 2499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2501 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.673' WHERE PROPTB_TUSS_ID = 2500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2502 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.730' WHERE PROPTB_TUSS_ID = 2501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2503 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.783' WHERE PROPTB_TUSS_ID = 2502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2504 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.840' WHERE PROPTB_TUSS_ID = 2503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2505 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.897' WHERE PROPTB_TUSS_ID = 2504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2506 , CLOUD_SYNC_DATE = '2014-05-06 17:36:37.947' WHERE PROPTB_TUSS_ID = 2505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2507 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.003' WHERE PROPTB_TUSS_ID = 2506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2508 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.060' WHERE PROPTB_TUSS_ID = 2507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2509 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.117' WHERE PROPTB_TUSS_ID = 2508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2510 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.170' WHERE PROPTB_TUSS_ID = 2509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2511 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.223' WHERE PROPTB_TUSS_ID = 2510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2512 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.280' WHERE PROPTB_TUSS_ID = 2511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2513 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.333' WHERE PROPTB_TUSS_ID = 2512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2514 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.387' WHERE PROPTB_TUSS_ID = 2513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2515 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.443' WHERE PROPTB_TUSS_ID = 2514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2516 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.497' WHERE PROPTB_TUSS_ID = 2515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2517 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.550' WHERE PROPTB_TUSS_ID = 2516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2518 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.607' WHERE PROPTB_TUSS_ID = 2517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2519 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.667' WHERE PROPTB_TUSS_ID = 2518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2520 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.717' WHERE PROPTB_TUSS_ID = 2519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2521 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.777' WHERE PROPTB_TUSS_ID = 2520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2522 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.830' WHERE PROPTB_TUSS_ID = 2521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2523 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.887' WHERE PROPTB_TUSS_ID = 2522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2524 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.937' WHERE PROPTB_TUSS_ID = 2523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2525 , CLOUD_SYNC_DATE = '2014-05-06 17:36:38.993' WHERE PROPTB_TUSS_ID = 2524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2526 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.050' WHERE PROPTB_TUSS_ID = 2525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2527 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.107' WHERE PROPTB_TUSS_ID = 2526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2528 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.160' WHERE PROPTB_TUSS_ID = 2527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2529 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.220' WHERE PROPTB_TUSS_ID = 2528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2530 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.273' WHERE PROPTB_TUSS_ID = 2529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2531 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.327' WHERE PROPTB_TUSS_ID = 2530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2532 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.380' WHERE PROPTB_TUSS_ID = 2531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2533 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.437' WHERE PROPTB_TUSS_ID = 2532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2534 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.487' WHERE PROPTB_TUSS_ID = 2533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2535 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.543' WHERE PROPTB_TUSS_ID = 2534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2536 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.600' WHERE PROPTB_TUSS_ID = 2535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2537 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.660' WHERE PROPTB_TUSS_ID = 2536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2538 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.717' WHERE PROPTB_TUSS_ID = 2537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2539 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.777' WHERE PROPTB_TUSS_ID = 2538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2540 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.833' WHERE PROPTB_TUSS_ID = 2539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2541 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.893' WHERE PROPTB_TUSS_ID = 2540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2542 , CLOUD_SYNC_DATE = '2014-05-06 17:36:39.947' WHERE PROPTB_TUSS_ID = 2541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2543 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.003' WHERE PROPTB_TUSS_ID = 2542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2544 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.063' WHERE PROPTB_TUSS_ID = 2543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2545 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.117' WHERE PROPTB_TUSS_ID = 2544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2546 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.173' WHERE PROPTB_TUSS_ID = 2545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2547 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.247' WHERE PROPTB_TUSS_ID = 2546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2548 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.320' WHERE PROPTB_TUSS_ID = 2547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2549 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.387' WHERE PROPTB_TUSS_ID = 2548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2550 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.467' WHERE PROPTB_TUSS_ID = 2549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2551 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.537' WHERE PROPTB_TUSS_ID = 2550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2552 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.600' WHERE PROPTB_TUSS_ID = 2551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2553 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.677' WHERE PROPTB_TUSS_ID = 2552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2554 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.753' WHERE PROPTB_TUSS_ID = 2553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2555 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.820' WHERE PROPTB_TUSS_ID = 2554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2556 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.883' WHERE PROPTB_TUSS_ID = 2555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2557 , CLOUD_SYNC_DATE = '2014-05-06 17:36:40.950' WHERE PROPTB_TUSS_ID = 2556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2558 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.033' WHERE PROPTB_TUSS_ID = 2557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2559 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.123' WHERE PROPTB_TUSS_ID = 2558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2560 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.190' WHERE PROPTB_TUSS_ID = 2559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2561 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.253' WHERE PROPTB_TUSS_ID = 2560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2562 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.313' WHERE PROPTB_TUSS_ID = 2561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2563 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.387' WHERE PROPTB_TUSS_ID = 2562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2564 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.467' WHERE PROPTB_TUSS_ID = 2563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2565 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.530' WHERE PROPTB_TUSS_ID = 2564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2566 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.590' WHERE PROPTB_TUSS_ID = 2565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2567 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.657' WHERE PROPTB_TUSS_ID = 2566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2568 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.717' WHERE PROPTB_TUSS_ID = 2567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2569 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.773' WHERE PROPTB_TUSS_ID = 2568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2570 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.837' WHERE PROPTB_TUSS_ID = 2569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2571 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.900' WHERE PROPTB_TUSS_ID = 2570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2572 , CLOUD_SYNC_DATE = '2014-05-06 17:36:41.957' WHERE PROPTB_TUSS_ID = 2571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2573 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.013' WHERE PROPTB_TUSS_ID = 2572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2574 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.070' WHERE PROPTB_TUSS_ID = 2573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2575 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.127' WHERE PROPTB_TUSS_ID = 2574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2576 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.183' WHERE PROPTB_TUSS_ID = 2575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2577 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.243' WHERE PROPTB_TUSS_ID = 2576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2578 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.307' WHERE PROPTB_TUSS_ID = 2577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2579 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.363' WHERE PROPTB_TUSS_ID = 2578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2580 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.417' WHERE PROPTB_TUSS_ID = 2579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2581 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.473' WHERE PROPTB_TUSS_ID = 2580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2582 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.530' WHERE PROPTB_TUSS_ID = 2581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2583 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.587' WHERE PROPTB_TUSS_ID = 2582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2584 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.640' WHERE PROPTB_TUSS_ID = 2583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2585 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.707' WHERE PROPTB_TUSS_ID = 2584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2586 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.763' WHERE PROPTB_TUSS_ID = 2585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2587 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.820' WHERE PROPTB_TUSS_ID = 2586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2588 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.880' WHERE PROPTB_TUSS_ID = 2587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2589 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.937' WHERE PROPTB_TUSS_ID = 2588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2590 , CLOUD_SYNC_DATE = '2014-05-06 17:36:42.993' WHERE PROPTB_TUSS_ID = 2589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2591 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.057' WHERE PROPTB_TUSS_ID = 2590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2592 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.127' WHERE PROPTB_TUSS_ID = 2591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2593 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.187' WHERE PROPTB_TUSS_ID = 2592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2594 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.240' WHERE PROPTB_TUSS_ID = 2593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2595 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.297' WHERE PROPTB_TUSS_ID = 2594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2596 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.353' WHERE PROPTB_TUSS_ID = 2595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2597 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.410' WHERE PROPTB_TUSS_ID = 2596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2598 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.467' WHERE PROPTB_TUSS_ID = 2597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2599 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.520' WHERE PROPTB_TUSS_ID = 2598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2600 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.577' WHERE PROPTB_TUSS_ID = 2599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2601 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.633' WHERE PROPTB_TUSS_ID = 2600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2602 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.697' WHERE PROPTB_TUSS_ID = 2601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2603 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.753' WHERE PROPTB_TUSS_ID = 2602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2604 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.807' WHERE PROPTB_TUSS_ID = 2603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2605 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.867' WHERE PROPTB_TUSS_ID = 2604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2606 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.923' WHERE PROPTB_TUSS_ID = 2605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2607 , CLOUD_SYNC_DATE = '2014-05-06 17:36:43.977' WHERE PROPTB_TUSS_ID = 2606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2608 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.033' WHERE PROPTB_TUSS_ID = 2607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2609 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.087' WHERE PROPTB_TUSS_ID = 2608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2610 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.143' WHERE PROPTB_TUSS_ID = 2609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2611 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.200' WHERE PROPTB_TUSS_ID = 2610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2612 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.257' WHERE PROPTB_TUSS_ID = 2611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2613 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.317' WHERE PROPTB_TUSS_ID = 2612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2614 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.373' WHERE PROPTB_TUSS_ID = 2613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2615 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.427' WHERE PROPTB_TUSS_ID = 2614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2616 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.480' WHERE PROPTB_TUSS_ID = 2615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2617 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.537' WHERE PROPTB_TUSS_ID = 2616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2618 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.597' WHERE PROPTB_TUSS_ID = 2617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2619 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.653' WHERE PROPTB_TUSS_ID = 2618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2620 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.710' WHERE PROPTB_TUSS_ID = 2619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2621 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.767' WHERE PROPTB_TUSS_ID = 2620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2622 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.823' WHERE PROPTB_TUSS_ID = 2621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2623 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.883' WHERE PROPTB_TUSS_ID = 2622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2624 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.940' WHERE PROPTB_TUSS_ID = 2623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2625 , CLOUD_SYNC_DATE = '2014-05-06 17:36:44.997' WHERE PROPTB_TUSS_ID = 2624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2626 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.053' WHERE PROPTB_TUSS_ID = 2625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2627 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.113' WHERE PROPTB_TUSS_ID = 2626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2628 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.173' WHERE PROPTB_TUSS_ID = 2627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2629 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.230' WHERE PROPTB_TUSS_ID = 2628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2630 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.297' WHERE PROPTB_TUSS_ID = 2629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2631 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.353' WHERE PROPTB_TUSS_ID = 2630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2632 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.407' WHERE PROPTB_TUSS_ID = 2631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2633 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.467' WHERE PROPTB_TUSS_ID = 2632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2634 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.520' WHERE PROPTB_TUSS_ID = 2633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2635 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.580' WHERE PROPTB_TUSS_ID = 2634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2636 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.640' WHERE PROPTB_TUSS_ID = 2635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2637 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.700' WHERE PROPTB_TUSS_ID = 2636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2638 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.757' WHERE PROPTB_TUSS_ID = 2637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2639 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.817' WHERE PROPTB_TUSS_ID = 2638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2640 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.877' WHERE PROPTB_TUSS_ID = 2639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2641 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.937' WHERE PROPTB_TUSS_ID = 2640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2642 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.997' WHERE PROPTB_TUSS_ID = 2641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2643 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.060' WHERE PROPTB_TUSS_ID = 2642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2644 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.120' WHERE PROPTB_TUSS_ID = 2643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2645 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.177' WHERE PROPTB_TUSS_ID = 2644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2646 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.237' WHERE PROPTB_TUSS_ID = 2645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2647 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.297' WHERE PROPTB_TUSS_ID = 2646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2648 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.357' WHERE PROPTB_TUSS_ID = 2647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2649 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.420' WHERE PROPTB_TUSS_ID = 2648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2650 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.480' WHERE PROPTB_TUSS_ID = 2649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2651 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.540' WHERE PROPTB_TUSS_ID = 2650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2652 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.597' WHERE PROPTB_TUSS_ID = 2651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2653 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.657' WHERE PROPTB_TUSS_ID = 2652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2654 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.717' WHERE PROPTB_TUSS_ID = 2653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2655 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.777' WHERE PROPTB_TUSS_ID = 2654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2656 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.837' WHERE PROPTB_TUSS_ID = 2655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2657 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.900' WHERE PROPTB_TUSS_ID = 2656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2658 , CLOUD_SYNC_DATE = '2014-05-06 17:36:46.960' WHERE PROPTB_TUSS_ID = 2657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2659 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.020' WHERE PROPTB_TUSS_ID = 2658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2660 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.077' WHERE PROPTB_TUSS_ID = 2659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2661 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.137' WHERE PROPTB_TUSS_ID = 2660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2662 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.200' WHERE PROPTB_TUSS_ID = 2661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2663 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.260' WHERE PROPTB_TUSS_ID = 2662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2664 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.317' WHERE PROPTB_TUSS_ID = 2663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2665 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.377' WHERE PROPTB_TUSS_ID = 2664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2666 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.447' WHERE PROPTB_TUSS_ID = 2665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2667 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.503' WHERE PROPTB_TUSS_ID = 2666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2668 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.567' WHERE PROPTB_TUSS_ID = 2667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2669 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.623' WHERE PROPTB_TUSS_ID = 2668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2670 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.687' WHERE PROPTB_TUSS_ID = 2669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2671 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.747' WHERE PROPTB_TUSS_ID = 2670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2672 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.807' WHERE PROPTB_TUSS_ID = 2671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2673 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.867' WHERE PROPTB_TUSS_ID = 2672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2674 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.927' WHERE PROPTB_TUSS_ID = 2673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2675 , CLOUD_SYNC_DATE = '2014-05-06 17:36:47.987' WHERE PROPTB_TUSS_ID = 2674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2676 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.043' WHERE PROPTB_TUSS_ID = 2675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2677 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.103' WHERE PROPTB_TUSS_ID = 2676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2678 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.163' WHERE PROPTB_TUSS_ID = 2677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2679 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.223' WHERE PROPTB_TUSS_ID = 2678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2680 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.283' WHERE PROPTB_TUSS_ID = 2679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2681 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.347' WHERE PROPTB_TUSS_ID = 2680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2682 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.407' WHERE PROPTB_TUSS_ID = 2681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2683 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.467' WHERE PROPTB_TUSS_ID = 2682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2684 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.527' WHERE PROPTB_TUSS_ID = 2683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2685 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.583' WHERE PROPTB_TUSS_ID = 2684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2686 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.647' WHERE PROPTB_TUSS_ID = 2685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2687 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.707' WHERE PROPTB_TUSS_ID = 2686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2688 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.767' WHERE PROPTB_TUSS_ID = 2687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2689 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.827' WHERE PROPTB_TUSS_ID = 2688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2690 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.887' WHERE PROPTB_TUSS_ID = 2689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2691 , CLOUD_SYNC_DATE = '2014-05-06 17:36:48.943' WHERE PROPTB_TUSS_ID = 2690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2692 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.000' WHERE PROPTB_TUSS_ID = 2691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2693 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.060' WHERE PROPTB_TUSS_ID = 2692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2694 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.120' WHERE PROPTB_TUSS_ID = 2693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2695 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.173' WHERE PROPTB_TUSS_ID = 2694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2696 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.230' WHERE PROPTB_TUSS_ID = 2695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2697 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.290' WHERE PROPTB_TUSS_ID = 2696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2698 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.347' WHERE PROPTB_TUSS_ID = 2697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2699 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.407' WHERE PROPTB_TUSS_ID = 2698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2700 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.470' WHERE PROPTB_TUSS_ID = 2699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2701 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.527' WHERE PROPTB_TUSS_ID = 2700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2702 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.590' WHERE PROPTB_TUSS_ID = 2701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2703 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.653' WHERE PROPTB_TUSS_ID = 2702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2704 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.717' WHERE PROPTB_TUSS_ID = 2703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2705 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.780' WHERE PROPTB_TUSS_ID = 2704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2706 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.837' WHERE PROPTB_TUSS_ID = 2705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2707 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.893' WHERE PROPTB_TUSS_ID = 2706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2708 , CLOUD_SYNC_DATE = '2014-05-06 17:36:49.953' WHERE PROPTB_TUSS_ID = 2707
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2709 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.013' WHERE PROPTB_TUSS_ID = 2708
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2710 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.070' WHERE PROPTB_TUSS_ID = 2709
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2711 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.130' WHERE PROPTB_TUSS_ID = 2710
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2712 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.190' WHERE PROPTB_TUSS_ID = 2711
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2713 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.250' WHERE PROPTB_TUSS_ID = 2712
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2714 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.310' WHERE PROPTB_TUSS_ID = 2713
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2715 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.370' WHERE PROPTB_TUSS_ID = 2714
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2716 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.430' WHERE PROPTB_TUSS_ID = 2715
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2717 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.490' WHERE PROPTB_TUSS_ID = 2716
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2718 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.550' WHERE PROPTB_TUSS_ID = 2717
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2719 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.613' WHERE PROPTB_TUSS_ID = 2718
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2720 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.677' WHERE PROPTB_TUSS_ID = 2719
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2721 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.737' WHERE PROPTB_TUSS_ID = 2720
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2722 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.797' WHERE PROPTB_TUSS_ID = 2721
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2723 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.857' WHERE PROPTB_TUSS_ID = 2722
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2724 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.913' WHERE PROPTB_TUSS_ID = 2723
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2725 , CLOUD_SYNC_DATE = '2014-05-06 17:36:50.973' WHERE PROPTB_TUSS_ID = 2724
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2726 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.033' WHERE PROPTB_TUSS_ID = 2725
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2727 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.090' WHERE PROPTB_TUSS_ID = 2726
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2728 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.150' WHERE PROPTB_TUSS_ID = 2727
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2729 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.210' WHERE PROPTB_TUSS_ID = 2728
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2730 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.270' WHERE PROPTB_TUSS_ID = 2729
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2731 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.330' WHERE PROPTB_TUSS_ID = 2730
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2732 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.387' WHERE PROPTB_TUSS_ID = 2731
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2733 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.447' WHERE PROPTB_TUSS_ID = 2732
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2734 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.507' WHERE PROPTB_TUSS_ID = 2733
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2735 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.567' WHERE PROPTB_TUSS_ID = 2734
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2736 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.623' WHERE PROPTB_TUSS_ID = 2735
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2737 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.687' WHERE PROPTB_TUSS_ID = 2736
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2738 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.750' WHERE PROPTB_TUSS_ID = 2737
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2739 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.813' WHERE PROPTB_TUSS_ID = 2738
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2740 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.890' WHERE PROPTB_TUSS_ID = 2739
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2741 , CLOUD_SYNC_DATE = '2014-05-06 17:36:51.977' WHERE PROPTB_TUSS_ID = 2740
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2742 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.040' WHERE PROPTB_TUSS_ID = 2741
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2743 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.100' WHERE PROPTB_TUSS_ID = 2742
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2744 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.160' WHERE PROPTB_TUSS_ID = 2743
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2745 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.220' WHERE PROPTB_TUSS_ID = 2744
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2746 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.277' WHERE PROPTB_TUSS_ID = 2745
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2747 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.337' WHERE PROPTB_TUSS_ID = 2746
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2748 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.397' WHERE PROPTB_TUSS_ID = 2747
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2749 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.457' WHERE PROPTB_TUSS_ID = 2748
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2750 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.517' WHERE PROPTB_TUSS_ID = 2749
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2751 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.577' WHERE PROPTB_TUSS_ID = 2750
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2752 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.637' WHERE PROPTB_TUSS_ID = 2751
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2753 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.697' WHERE PROPTB_TUSS_ID = 2752
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2754 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.757' WHERE PROPTB_TUSS_ID = 2753
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2755 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.817' WHERE PROPTB_TUSS_ID = 2754
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2756 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.893' WHERE PROPTB_TUSS_ID = 2755
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2757 , CLOUD_SYNC_DATE = '2014-05-06 17:36:52.957' WHERE PROPTB_TUSS_ID = 2756
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2758 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.017' WHERE PROPTB_TUSS_ID = 2757
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2759 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.080' WHERE PROPTB_TUSS_ID = 2758
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2760 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.140' WHERE PROPTB_TUSS_ID = 2759
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2761 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.200' WHERE PROPTB_TUSS_ID = 2760
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2762 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.260' WHERE PROPTB_TUSS_ID = 2761
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2763 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.320' WHERE PROPTB_TUSS_ID = 2762
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2764 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.380' WHERE PROPTB_TUSS_ID = 2763
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2765 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.443' WHERE PROPTB_TUSS_ID = 2764
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2766 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.503' WHERE PROPTB_TUSS_ID = 2765
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2767 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.567' WHERE PROPTB_TUSS_ID = 2766
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2768 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.627' WHERE PROPTB_TUSS_ID = 2767
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2769 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.693' WHERE PROPTB_TUSS_ID = 2768
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2770 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.757' WHERE PROPTB_TUSS_ID = 2769
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2771 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.823' WHERE PROPTB_TUSS_ID = 2770
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2772 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.893' WHERE PROPTB_TUSS_ID = 2771
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2773 , CLOUD_SYNC_DATE = '2014-05-06 17:36:53.957' WHERE PROPTB_TUSS_ID = 2772
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2774 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.017' WHERE PROPTB_TUSS_ID = 2773
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2775 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.080' WHERE PROPTB_TUSS_ID = 2774
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2776 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.147' WHERE PROPTB_TUSS_ID = 2775
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2777 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.213' WHERE PROPTB_TUSS_ID = 2776
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2778 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.280' WHERE PROPTB_TUSS_ID = 2777
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2779 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.347' WHERE PROPTB_TUSS_ID = 2778
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2780 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.410' WHERE PROPTB_TUSS_ID = 2779
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2781 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.473' WHERE PROPTB_TUSS_ID = 2780
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2782 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.533' WHERE PROPTB_TUSS_ID = 2781
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2783 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.593' WHERE PROPTB_TUSS_ID = 2782
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2784 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.657' WHERE PROPTB_TUSS_ID = 2783
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2785 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.717' WHERE PROPTB_TUSS_ID = 2784
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2786 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.780' WHERE PROPTB_TUSS_ID = 2785
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2787 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.860' WHERE PROPTB_TUSS_ID = 2786
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2788 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.920' WHERE PROPTB_TUSS_ID = 2787
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2789 , CLOUD_SYNC_DATE = '2014-05-06 17:36:54.983' WHERE PROPTB_TUSS_ID = 2788
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2790 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.047' WHERE PROPTB_TUSS_ID = 2789
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2791 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.107' WHERE PROPTB_TUSS_ID = 2790
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2792 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.167' WHERE PROPTB_TUSS_ID = 2791
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2793 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.230' WHERE PROPTB_TUSS_ID = 2792
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2794 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.290' WHERE PROPTB_TUSS_ID = 2793
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2795 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.350' WHERE PROPTB_TUSS_ID = 2794
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2796 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.413' WHERE PROPTB_TUSS_ID = 2795
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2797 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.473' WHERE PROPTB_TUSS_ID = 2796
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2798 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.537' WHERE PROPTB_TUSS_ID = 2797
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2799 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.597' WHERE PROPTB_TUSS_ID = 2798
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2800 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.657' WHERE PROPTB_TUSS_ID = 2799
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2801 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.720' WHERE PROPTB_TUSS_ID = 2800
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2802 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.780' WHERE PROPTB_TUSS_ID = 2801
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2803 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.847' WHERE PROPTB_TUSS_ID = 2802
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2804 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.907' WHERE PROPTB_TUSS_ID = 2803
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2805 , CLOUD_SYNC_DATE = '2014-05-06 17:36:55.967' WHERE PROPTB_TUSS_ID = 2804
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2806 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.030' WHERE PROPTB_TUSS_ID = 2805
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2807 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.093' WHERE PROPTB_TUSS_ID = 2806
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2808 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.157' WHERE PROPTB_TUSS_ID = 2807
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2809 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.217' WHERE PROPTB_TUSS_ID = 2808
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2810 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.277' WHERE PROPTB_TUSS_ID = 2809
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2811 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.340' WHERE PROPTB_TUSS_ID = 2810
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2812 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.397' WHERE PROPTB_TUSS_ID = 2811
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2813 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.460' WHERE PROPTB_TUSS_ID = 2812
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2814 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.520' WHERE PROPTB_TUSS_ID = 2813
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2815 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.580' WHERE PROPTB_TUSS_ID = 2814
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2816 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.643' WHERE PROPTB_TUSS_ID = 2815
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2817 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.707' WHERE PROPTB_TUSS_ID = 2816
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2818 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.770' WHERE PROPTB_TUSS_ID = 2817
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2819 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.833' WHERE PROPTB_TUSS_ID = 2818
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2820 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.897' WHERE PROPTB_TUSS_ID = 2819
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2821 , CLOUD_SYNC_DATE = '2014-05-06 17:36:56.960' WHERE PROPTB_TUSS_ID = 2820
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2822 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.023' WHERE PROPTB_TUSS_ID = 2821
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2823 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.087' WHERE PROPTB_TUSS_ID = 2822
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2824 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.147' WHERE PROPTB_TUSS_ID = 2823
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2825 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.210' WHERE PROPTB_TUSS_ID = 2824
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2826 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.270' WHERE PROPTB_TUSS_ID = 2825
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2827 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.337' WHERE PROPTB_TUSS_ID = 2826
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2828 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.413' WHERE PROPTB_TUSS_ID = 2827
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2829 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.477' WHERE PROPTB_TUSS_ID = 2828
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2830 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.537' WHERE PROPTB_TUSS_ID = 2829
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2831 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.597' WHERE PROPTB_TUSS_ID = 2830
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2832 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.667' WHERE PROPTB_TUSS_ID = 2831
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2833 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.727' WHERE PROPTB_TUSS_ID = 2832
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2834 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.793' WHERE PROPTB_TUSS_ID = 2833
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2835 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.857' WHERE PROPTB_TUSS_ID = 2834
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2836 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.920' WHERE PROPTB_TUSS_ID = 2835
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2837 , CLOUD_SYNC_DATE = '2014-05-06 17:36:57.987' WHERE PROPTB_TUSS_ID = 2836
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2838 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.047' WHERE PROPTB_TUSS_ID = 2837
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2839 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.113' WHERE PROPTB_TUSS_ID = 2838
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2840 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.177' WHERE PROPTB_TUSS_ID = 2839
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2841 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.240' WHERE PROPTB_TUSS_ID = 2840
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2842 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.303' WHERE PROPTB_TUSS_ID = 2841
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2843 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.363' WHERE PROPTB_TUSS_ID = 2842
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2844 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.430' WHERE PROPTB_TUSS_ID = 2843
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2845 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.493' WHERE PROPTB_TUSS_ID = 2844
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2846 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.570' WHERE PROPTB_TUSS_ID = 2845
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2847 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.637' WHERE PROPTB_TUSS_ID = 2846
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2848 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.703' WHERE PROPTB_TUSS_ID = 2847
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2849 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.763' WHERE PROPTB_TUSS_ID = 2848
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2850 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.827' WHERE PROPTB_TUSS_ID = 2849
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2851 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.893' WHERE PROPTB_TUSS_ID = 2850
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2852 , CLOUD_SYNC_DATE = '2014-05-06 17:36:58.957' WHERE PROPTB_TUSS_ID = 2851
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2853 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.023' WHERE PROPTB_TUSS_ID = 2852
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2854 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.087' WHERE PROPTB_TUSS_ID = 2853
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2855 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.150' WHERE PROPTB_TUSS_ID = 2854
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2856 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.213' WHERE PROPTB_TUSS_ID = 2855
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2857 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.277' WHERE PROPTB_TUSS_ID = 2856
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2858 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.337' WHERE PROPTB_TUSS_ID = 2857
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2859 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.403' WHERE PROPTB_TUSS_ID = 2858
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2860 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.467' WHERE PROPTB_TUSS_ID = 2859
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2861 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.527' WHERE PROPTB_TUSS_ID = 2860
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2862 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.590' WHERE PROPTB_TUSS_ID = 2861
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2863 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.667' WHERE PROPTB_TUSS_ID = 2862
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2864 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.737' WHERE PROPTB_TUSS_ID = 2863
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2865 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.807' WHERE PROPTB_TUSS_ID = 2864
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2866 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.867' WHERE PROPTB_TUSS_ID = 2865
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2867 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.930' WHERE PROPTB_TUSS_ID = 2866
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2868 , CLOUD_SYNC_DATE = '2014-05-06 17:36:59.993' WHERE PROPTB_TUSS_ID = 2867
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2869 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.057' WHERE PROPTB_TUSS_ID = 2868
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2870 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.123' WHERE PROPTB_TUSS_ID = 2869
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2871 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.183' WHERE PROPTB_TUSS_ID = 2870
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2872 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.247' WHERE PROPTB_TUSS_ID = 2871
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2873 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.307' WHERE PROPTB_TUSS_ID = 2872
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2874 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.370' WHERE PROPTB_TUSS_ID = 2873
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2875 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.450' WHERE PROPTB_TUSS_ID = 2874
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2876 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.527' WHERE PROPTB_TUSS_ID = 2875
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2877 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.590' WHERE PROPTB_TUSS_ID = 2876
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2878 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.657' WHERE PROPTB_TUSS_ID = 2877
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2879 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.720' WHERE PROPTB_TUSS_ID = 2878
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2880 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.787' WHERE PROPTB_TUSS_ID = 2879
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2881 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.850' WHERE PROPTB_TUSS_ID = 2880
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2882 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.913' WHERE PROPTB_TUSS_ID = 2881
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2883 , CLOUD_SYNC_DATE = '2014-05-06 17:37:00.977' WHERE PROPTB_TUSS_ID = 2882
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2884 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.040' WHERE PROPTB_TUSS_ID = 2883
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2885 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.103' WHERE PROPTB_TUSS_ID = 2884
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2886 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.170' WHERE PROPTB_TUSS_ID = 2885
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2887 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.233' WHERE PROPTB_TUSS_ID = 2886
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2888 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.297' WHERE PROPTB_TUSS_ID = 2887
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2889 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.363' WHERE PROPTB_TUSS_ID = 2888
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2890 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.423' WHERE PROPTB_TUSS_ID = 2889
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2891 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.487' WHERE PROPTB_TUSS_ID = 2890
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2892 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.550' WHERE PROPTB_TUSS_ID = 2891
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2893 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.613' WHERE PROPTB_TUSS_ID = 2892
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2894 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.677' WHERE PROPTB_TUSS_ID = 2893
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2895 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.743' WHERE PROPTB_TUSS_ID = 2894
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2896 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.807' WHERE PROPTB_TUSS_ID = 2895
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2897 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.877' WHERE PROPTB_TUSS_ID = 2896
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2898 , CLOUD_SYNC_DATE = '2014-05-06 17:37:01.943' WHERE PROPTB_TUSS_ID = 2897
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2899 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.017' WHERE PROPTB_TUSS_ID = 2898
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2900 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.097' WHERE PROPTB_TUSS_ID = 2899
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2901 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.163' WHERE PROPTB_TUSS_ID = 2900
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2902 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.230' WHERE PROPTB_TUSS_ID = 2901
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2903 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.293' WHERE PROPTB_TUSS_ID = 2902
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2904 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.357' WHERE PROPTB_TUSS_ID = 2903
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2905 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.427' WHERE PROPTB_TUSS_ID = 2904
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2906 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.490' WHERE PROPTB_TUSS_ID = 2905
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2907 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.550' WHERE PROPTB_TUSS_ID = 2906
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2908 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.613' WHERE PROPTB_TUSS_ID = 2907
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2909 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.687' WHERE PROPTB_TUSS_ID = 2908
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2910 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.770' WHERE PROPTB_TUSS_ID = 2909
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2911 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.833' WHERE PROPTB_TUSS_ID = 2910
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2912 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.897' WHERE PROPTB_TUSS_ID = 2911
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2913 , CLOUD_SYNC_DATE = '2014-05-06 17:37:02.967' WHERE PROPTB_TUSS_ID = 2912
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2914 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.037' WHERE PROPTB_TUSS_ID = 2913
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2915 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.110' WHERE PROPTB_TUSS_ID = 2914
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2916 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.190' WHERE PROPTB_TUSS_ID = 2915
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2917 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.253' WHERE PROPTB_TUSS_ID = 2916
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2918 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.317' WHERE PROPTB_TUSS_ID = 2917
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2919 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.383' WHERE PROPTB_TUSS_ID = 2918
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2920 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.447' WHERE PROPTB_TUSS_ID = 2919
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2921 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.507' WHERE PROPTB_TUSS_ID = 2920
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2922 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.570' WHERE PROPTB_TUSS_ID = 2921
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2923 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.633' WHERE PROPTB_TUSS_ID = 2922
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2924 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.697' WHERE PROPTB_TUSS_ID = 2923
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2925 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.760' WHERE PROPTB_TUSS_ID = 2924
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2926 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.827' WHERE PROPTB_TUSS_ID = 2925
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2927 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.890' WHERE PROPTB_TUSS_ID = 2926
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2928 , CLOUD_SYNC_DATE = '2014-05-06 17:37:03.953' WHERE PROPTB_TUSS_ID = 2927
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2929 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.017' WHERE PROPTB_TUSS_ID = 2928
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2930 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.083' WHERE PROPTB_TUSS_ID = 2929
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2931 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.147' WHERE PROPTB_TUSS_ID = 2930
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2932 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.217' WHERE PROPTB_TUSS_ID = 2931
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2933 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.277' WHERE PROPTB_TUSS_ID = 2932
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2934 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.343' WHERE PROPTB_TUSS_ID = 2933
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2935 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.407' WHERE PROPTB_TUSS_ID = 2934
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2936 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.467' WHERE PROPTB_TUSS_ID = 2935
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2937 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.530' WHERE PROPTB_TUSS_ID = 2936
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2938 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.597' WHERE PROPTB_TUSS_ID = 2937
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2939 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.663' WHERE PROPTB_TUSS_ID = 2938
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2940 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.727' WHERE PROPTB_TUSS_ID = 2939
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2941 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.790' WHERE PROPTB_TUSS_ID = 2940
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2942 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.857' WHERE PROPTB_TUSS_ID = 2941
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2943 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.923' WHERE PROPTB_TUSS_ID = 2942
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2944 , CLOUD_SYNC_DATE = '2014-05-06 17:37:04.993' WHERE PROPTB_TUSS_ID = 2943
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2945 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.057' WHERE PROPTB_TUSS_ID = 2944
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2946 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.120' WHERE PROPTB_TUSS_ID = 2945
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2947 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.183' WHERE PROPTB_TUSS_ID = 2946
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2948 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.247' WHERE PROPTB_TUSS_ID = 2947
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2949 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.317' WHERE PROPTB_TUSS_ID = 2948
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2950 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.383' WHERE PROPTB_TUSS_ID = 2949
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2951 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.447' WHERE PROPTB_TUSS_ID = 2950
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2952 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.510' WHERE PROPTB_TUSS_ID = 2951
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2953 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.573' WHERE PROPTB_TUSS_ID = 2952
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2954 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.640' WHERE PROPTB_TUSS_ID = 2953
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2955 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.713' WHERE PROPTB_TUSS_ID = 2954
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2956 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.777' WHERE PROPTB_TUSS_ID = 2955
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2957 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.840' WHERE PROPTB_TUSS_ID = 2956
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2958 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.903' WHERE PROPTB_TUSS_ID = 2957
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2959 , CLOUD_SYNC_DATE = '2014-05-06 17:37:05.967' WHERE PROPTB_TUSS_ID = 2958
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2960 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.033' WHERE PROPTB_TUSS_ID = 2959
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2961 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.097' WHERE PROPTB_TUSS_ID = 2960
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2962 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.160' WHERE PROPTB_TUSS_ID = 2961
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2963 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.223' WHERE PROPTB_TUSS_ID = 2962
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2964 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.287' WHERE PROPTB_TUSS_ID = 2963
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2965 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.353' WHERE PROPTB_TUSS_ID = 2964
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2966 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.417' WHERE PROPTB_TUSS_ID = 2965
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2967 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.480' WHERE PROPTB_TUSS_ID = 2966
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2968 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.543' WHERE PROPTB_TUSS_ID = 2967
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2969 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.607' WHERE PROPTB_TUSS_ID = 2968
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2970 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.677' WHERE PROPTB_TUSS_ID = 2969
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2971 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.743' WHERE PROPTB_TUSS_ID = 2970
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2972 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.807' WHERE PROPTB_TUSS_ID = 2971
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2973 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.870' WHERE PROPTB_TUSS_ID = 2972
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2974 , CLOUD_SYNC_DATE = '2014-05-06 17:37:06.937' WHERE PROPTB_TUSS_ID = 2973
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2975 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.017' WHERE PROPTB_TUSS_ID = 2974
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2976 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.133' WHERE PROPTB_TUSS_ID = 2975
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2977 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.247' WHERE PROPTB_TUSS_ID = 2976
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2978 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.333' WHERE PROPTB_TUSS_ID = 2977
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2979 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.443' WHERE PROPTB_TUSS_ID = 2978
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2980 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.550' WHERE PROPTB_TUSS_ID = 2979
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2981 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.620' WHERE PROPTB_TUSS_ID = 2980
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2982 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.683' WHERE PROPTB_TUSS_ID = 2981
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2983 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.750' WHERE PROPTB_TUSS_ID = 2982
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2984 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.817' WHERE PROPTB_TUSS_ID = 2983
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2985 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.883' WHERE PROPTB_TUSS_ID = 2984
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2986 , CLOUD_SYNC_DATE = '2014-05-06 17:37:07.960' WHERE PROPTB_TUSS_ID = 2985
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2987 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.027' WHERE PROPTB_TUSS_ID = 2986
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2988 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.093' WHERE PROPTB_TUSS_ID = 2987
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2989 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.157' WHERE PROPTB_TUSS_ID = 2988
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2990 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.220' WHERE PROPTB_TUSS_ID = 2989
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2991 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.283' WHERE PROPTB_TUSS_ID = 2990
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2992 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.350' WHERE PROPTB_TUSS_ID = 2991
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2993 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.417' WHERE PROPTB_TUSS_ID = 2992
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2994 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.480' WHERE PROPTB_TUSS_ID = 2993
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2995 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.543' WHERE PROPTB_TUSS_ID = 2994
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2996 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.613' WHERE PROPTB_TUSS_ID = 2995
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2997 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.680' WHERE PROPTB_TUSS_ID = 2996
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2998 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.747' WHERE PROPTB_TUSS_ID = 2997
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2999 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.807' WHERE PROPTB_TUSS_ID = 2998
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3000 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.873' WHERE PROPTB_TUSS_ID = 2999
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3001 , CLOUD_SYNC_DATE = '2014-05-06 17:37:08.943' WHERE PROPTB_TUSS_ID = 3000
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3002 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.020' WHERE PROPTB_TUSS_ID = 3001
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3003 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.093' WHERE PROPTB_TUSS_ID = 3002
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3004 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.157' WHERE PROPTB_TUSS_ID = 3003
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3005 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.227' WHERE PROPTB_TUSS_ID = 3004
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3006 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.290' WHERE PROPTB_TUSS_ID = 3005
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3007 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.353' WHERE PROPTB_TUSS_ID = 3006
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3008 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.420' WHERE PROPTB_TUSS_ID = 3007
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3009 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.487' WHERE PROPTB_TUSS_ID = 3008
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3010 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.553' WHERE PROPTB_TUSS_ID = 3009
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3011 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.617' WHERE PROPTB_TUSS_ID = 3010
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3012 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.690' WHERE PROPTB_TUSS_ID = 3011
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3013 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.760' WHERE PROPTB_TUSS_ID = 3012
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3014 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.830' WHERE PROPTB_TUSS_ID = 3013
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3015 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.897' WHERE PROPTB_TUSS_ID = 3014
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3016 , CLOUD_SYNC_DATE = '2014-05-06 17:37:09.960' WHERE PROPTB_TUSS_ID = 3015
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3017 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.030' WHERE PROPTB_TUSS_ID = 3016
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3018 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.097' WHERE PROPTB_TUSS_ID = 3017
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3019 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.163' WHERE PROPTB_TUSS_ID = 3018
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3020 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.227' WHERE PROPTB_TUSS_ID = 3019
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3021 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.297' WHERE PROPTB_TUSS_ID = 3020
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3022 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.360' WHERE PROPTB_TUSS_ID = 3021
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3023 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.427' WHERE PROPTB_TUSS_ID = 3022
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3024 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.497' WHERE PROPTB_TUSS_ID = 3023
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3025 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.560' WHERE PROPTB_TUSS_ID = 3024
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3026 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.627' WHERE PROPTB_TUSS_ID = 3025
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3027 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.697' WHERE PROPTB_TUSS_ID = 3026
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3028 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.763' WHERE PROPTB_TUSS_ID = 3027
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3029 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.830' WHERE PROPTB_TUSS_ID = 3028
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3030 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.903' WHERE PROPTB_TUSS_ID = 3029
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3031 , CLOUD_SYNC_DATE = '2014-05-06 17:37:10.967' WHERE PROPTB_TUSS_ID = 3030
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3032 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.037' WHERE PROPTB_TUSS_ID = 3031
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3033 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.103' WHERE PROPTB_TUSS_ID = 3032
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3034 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.170' WHERE PROPTB_TUSS_ID = 3033
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3035 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.240' WHERE PROPTB_TUSS_ID = 3034
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3036 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.307' WHERE PROPTB_TUSS_ID = 3035
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3037 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.373' WHERE PROPTB_TUSS_ID = 3036
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3038 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.440' WHERE PROPTB_TUSS_ID = 3037
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3039 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.510' WHERE PROPTB_TUSS_ID = 3038
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3040 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.573' WHERE PROPTB_TUSS_ID = 3039
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3041 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.640' WHERE PROPTB_TUSS_ID = 3040
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3042 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.707' WHERE PROPTB_TUSS_ID = 3041
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3043 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.777' WHERE PROPTB_TUSS_ID = 3042
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3044 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.847' WHERE PROPTB_TUSS_ID = 3043
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3045 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.913' WHERE PROPTB_TUSS_ID = 3044
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3046 , CLOUD_SYNC_DATE = '2014-05-06 17:37:11.983' WHERE PROPTB_TUSS_ID = 3045
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3047 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.047' WHERE PROPTB_TUSS_ID = 3046
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3048 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.117' WHERE PROPTB_TUSS_ID = 3047
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3049 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.190' WHERE PROPTB_TUSS_ID = 3048
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3050 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.257' WHERE PROPTB_TUSS_ID = 3049
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3051 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.323' WHERE PROPTB_TUSS_ID = 3050
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3052 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.390' WHERE PROPTB_TUSS_ID = 3051
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3053 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.460' WHERE PROPTB_TUSS_ID = 3052
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3054 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.527' WHERE PROPTB_TUSS_ID = 3053
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3055 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.593' WHERE PROPTB_TUSS_ID = 3054
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3056 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.670' WHERE PROPTB_TUSS_ID = 3055
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3057 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.733' WHERE PROPTB_TUSS_ID = 3056
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3058 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.803' WHERE PROPTB_TUSS_ID = 3057
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3059 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.870' WHERE PROPTB_TUSS_ID = 3058
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3060 , CLOUD_SYNC_DATE = '2014-05-06 17:37:12.947' WHERE PROPTB_TUSS_ID = 3059
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3061 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.013' WHERE PROPTB_TUSS_ID = 3060
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3062 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.080' WHERE PROPTB_TUSS_ID = 3061
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3063 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.150' WHERE PROPTB_TUSS_ID = 3062
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3064 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.217' WHERE PROPTB_TUSS_ID = 3063
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3065 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.283' WHERE PROPTB_TUSS_ID = 3064
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3066 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.350' WHERE PROPTB_TUSS_ID = 3065
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3067 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.417' WHERE PROPTB_TUSS_ID = 3066
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3068 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.487' WHERE PROPTB_TUSS_ID = 3067
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3069 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.567' WHERE PROPTB_TUSS_ID = 3068
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3070 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.637' WHERE PROPTB_TUSS_ID = 3069
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3071 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.710' WHERE PROPTB_TUSS_ID = 3070
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3072 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.777' WHERE PROPTB_TUSS_ID = 3071
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3073 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.847' WHERE PROPTB_TUSS_ID = 3072
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3074 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.913' WHERE PROPTB_TUSS_ID = 3073
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3075 , CLOUD_SYNC_DATE = '2014-05-06 17:37:13.980' WHERE PROPTB_TUSS_ID = 3074
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3076 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.050' WHERE PROPTB_TUSS_ID = 3075
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3077 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.127' WHERE PROPTB_TUSS_ID = 3076
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3078 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.193' WHERE PROPTB_TUSS_ID = 3077
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3079 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.267' WHERE PROPTB_TUSS_ID = 3078
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3080 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.337' WHERE PROPTB_TUSS_ID = 3079
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3081 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.403' WHERE PROPTB_TUSS_ID = 3080
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3082 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.477' WHERE PROPTB_TUSS_ID = 3081
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3083 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.547' WHERE PROPTB_TUSS_ID = 3082
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3084 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.617' WHERE PROPTB_TUSS_ID = 3083
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3085 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.697' WHERE PROPTB_TUSS_ID = 3084
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3086 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.767' WHERE PROPTB_TUSS_ID = 3085
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3087 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.830' WHERE PROPTB_TUSS_ID = 3086
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3088 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.903' WHERE PROPTB_TUSS_ID = 3087
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3089 , CLOUD_SYNC_DATE = '2014-05-06 17:37:14.970' WHERE PROPTB_TUSS_ID = 3088
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3090 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.063' WHERE PROPTB_TUSS_ID = 3089
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3091 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.137' WHERE PROPTB_TUSS_ID = 3090
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3092 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.203' WHERE PROPTB_TUSS_ID = 3091
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3093 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.267' WHERE PROPTB_TUSS_ID = 3092
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3094 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.337' WHERE PROPTB_TUSS_ID = 3093
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3095 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.403' WHERE PROPTB_TUSS_ID = 3094
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3096 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.470' WHERE PROPTB_TUSS_ID = 3095
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3097 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.540' WHERE PROPTB_TUSS_ID = 3096
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3098 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.607' WHERE PROPTB_TUSS_ID = 3097
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3099 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.677' WHERE PROPTB_TUSS_ID = 3098
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3100 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.747' WHERE PROPTB_TUSS_ID = 3099
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3101 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.810' WHERE PROPTB_TUSS_ID = 3100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3102 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.880' WHERE PROPTB_TUSS_ID = 3101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3103 , CLOUD_SYNC_DATE = '2014-05-06 17:37:15.953' WHERE PROPTB_TUSS_ID = 3102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3104 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.020' WHERE PROPTB_TUSS_ID = 3103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3105 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.087' WHERE PROPTB_TUSS_ID = 3104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3106 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.157' WHERE PROPTB_TUSS_ID = 3105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3107 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.223' WHERE PROPTB_TUSS_ID = 3106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3108 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.293' WHERE PROPTB_TUSS_ID = 3107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3109 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.360' WHERE PROPTB_TUSS_ID = 3108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3110 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.427' WHERE PROPTB_TUSS_ID = 3109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3111 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.497' WHERE PROPTB_TUSS_ID = 3110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3112 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.563' WHERE PROPTB_TUSS_ID = 3111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3113 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.630' WHERE PROPTB_TUSS_ID = 3112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3114 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.707' WHERE PROPTB_TUSS_ID = 3113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3115 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.773' WHERE PROPTB_TUSS_ID = 3114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3116 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.837' WHERE PROPTB_TUSS_ID = 3115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3117 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.913' WHERE PROPTB_TUSS_ID = 3116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3118 , CLOUD_SYNC_DATE = '2014-05-06 17:37:16.980' WHERE PROPTB_TUSS_ID = 3117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3119 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.047' WHERE PROPTB_TUSS_ID = 3118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3120 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.117' WHERE PROPTB_TUSS_ID = 3119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3121 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.180' WHERE PROPTB_TUSS_ID = 3120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3122 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.250' WHERE PROPTB_TUSS_ID = 3121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3123 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.317' WHERE PROPTB_TUSS_ID = 3122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3124 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.383' WHERE PROPTB_TUSS_ID = 3123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3125 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.453' WHERE PROPTB_TUSS_ID = 3124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3126 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.520' WHERE PROPTB_TUSS_ID = 3125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3127 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.587' WHERE PROPTB_TUSS_ID = 3126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3128 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.660' WHERE PROPTB_TUSS_ID = 3127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3129 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.727' WHERE PROPTB_TUSS_ID = 3128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3130 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.797' WHERE PROPTB_TUSS_ID = 3129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3131 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.863' WHERE PROPTB_TUSS_ID = 3130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3132 , CLOUD_SYNC_DATE = '2014-05-06 17:37:17.930' WHERE PROPTB_TUSS_ID = 3131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3133 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.003' WHERE PROPTB_TUSS_ID = 3132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3134 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.070' WHERE PROPTB_TUSS_ID = 3133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3135 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.137' WHERE PROPTB_TUSS_ID = 3134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3136 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.210' WHERE PROPTB_TUSS_ID = 3135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3137 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.277' WHERE PROPTB_TUSS_ID = 3136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3138 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.347' WHERE PROPTB_TUSS_ID = 3137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3139 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.417' WHERE PROPTB_TUSS_ID = 3138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3140 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.483' WHERE PROPTB_TUSS_ID = 3139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3141 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.553' WHERE PROPTB_TUSS_ID = 3140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3142 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.620' WHERE PROPTB_TUSS_ID = 3141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3143 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.693' WHERE PROPTB_TUSS_ID = 3142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3144 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.763' WHERE PROPTB_TUSS_ID = 3143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3145 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.830' WHERE PROPTB_TUSS_ID = 3144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3146 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.900' WHERE PROPTB_TUSS_ID = 3145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3147 , CLOUD_SYNC_DATE = '2014-05-06 17:37:18.970' WHERE PROPTB_TUSS_ID = 3146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3148 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.037' WHERE PROPTB_TUSS_ID = 3147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3149 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.107' WHERE PROPTB_TUSS_ID = 3148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3150 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.177' WHERE PROPTB_TUSS_ID = 3149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3151 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.247' WHERE PROPTB_TUSS_ID = 3150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3152 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.313' WHERE PROPTB_TUSS_ID = 3151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3153 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.380' WHERE PROPTB_TUSS_ID = 3152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3154 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.450' WHERE PROPTB_TUSS_ID = 3153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3155 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.520' WHERE PROPTB_TUSS_ID = 3154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3156 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.590' WHERE PROPTB_TUSS_ID = 3155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3157 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.660' WHERE PROPTB_TUSS_ID = 3156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3158 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.727' WHERE PROPTB_TUSS_ID = 3157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3159 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.800' WHERE PROPTB_TUSS_ID = 3158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3160 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.867' WHERE PROPTB_TUSS_ID = 3159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3161 , CLOUD_SYNC_DATE = '2014-05-06 17:37:19.940' WHERE PROPTB_TUSS_ID = 3160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3162 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.007' WHERE PROPTB_TUSS_ID = 3161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3163 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.080' WHERE PROPTB_TUSS_ID = 3162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3164 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.147' WHERE PROPTB_TUSS_ID = 3163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3165 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.217' WHERE PROPTB_TUSS_ID = 3164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3166 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.287' WHERE PROPTB_TUSS_ID = 3165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3167 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.357' WHERE PROPTB_TUSS_ID = 3166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3168 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.427' WHERE PROPTB_TUSS_ID = 3167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3169 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.493' WHERE PROPTB_TUSS_ID = 3168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3170 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.560' WHERE PROPTB_TUSS_ID = 3169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3171 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.633' WHERE PROPTB_TUSS_ID = 3170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3172 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.703' WHERE PROPTB_TUSS_ID = 3171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3173 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.773' WHERE PROPTB_TUSS_ID = 3172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3174 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.840' WHERE PROPTB_TUSS_ID = 3173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3175 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.913' WHERE PROPTB_TUSS_ID = 3174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3176 , CLOUD_SYNC_DATE = '2014-05-06 17:37:20.980' WHERE PROPTB_TUSS_ID = 3175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3177 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.050' WHERE PROPTB_TUSS_ID = 3176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3178 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.120' WHERE PROPTB_TUSS_ID = 3177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3179 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.193' WHERE PROPTB_TUSS_ID = 3178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3180 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.263' WHERE PROPTB_TUSS_ID = 3179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3181 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.360' WHERE PROPTB_TUSS_ID = 3180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3182 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.400' WHERE PROPTB_TUSS_ID = 3181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3183 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.470' WHERE PROPTB_TUSS_ID = 3182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3184 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.537' WHERE PROPTB_TUSS_ID = 3183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3185 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.610' WHERE PROPTB_TUSS_ID = 3184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3186 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.683' WHERE PROPTB_TUSS_ID = 3185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3187 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.757' WHERE PROPTB_TUSS_ID = 3186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3188 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.823' WHERE PROPTB_TUSS_ID = 3187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3189 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.897' WHERE PROPTB_TUSS_ID = 3188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3190 , CLOUD_SYNC_DATE = '2014-05-06 17:37:21.967' WHERE PROPTB_TUSS_ID = 3189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3191 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.033' WHERE PROPTB_TUSS_ID = 3190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3192 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.117' WHERE PROPTB_TUSS_ID = 3191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3193 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.197' WHERE PROPTB_TUSS_ID = 3192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3194 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.273' WHERE PROPTB_TUSS_ID = 3193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3195 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.347' WHERE PROPTB_TUSS_ID = 3194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3196 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.417' WHERE PROPTB_TUSS_ID = 3195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3197 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.487' WHERE PROPTB_TUSS_ID = 3196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3198 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.557' WHERE PROPTB_TUSS_ID = 3197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3199 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.627' WHERE PROPTB_TUSS_ID = 3198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3200 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.693' WHERE PROPTB_TUSS_ID = 3199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3201 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.767' WHERE PROPTB_TUSS_ID = 3200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3202 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.833' WHERE PROPTB_TUSS_ID = 3201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3203 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.907' WHERE PROPTB_TUSS_ID = 3202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3204 , CLOUD_SYNC_DATE = '2014-05-06 17:37:22.977' WHERE PROPTB_TUSS_ID = 3203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3205 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.047' WHERE PROPTB_TUSS_ID = 3204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3206 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.117' WHERE PROPTB_TUSS_ID = 3205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3207 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.190' WHERE PROPTB_TUSS_ID = 3206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3208 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.257' WHERE PROPTB_TUSS_ID = 3207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3209 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.327' WHERE PROPTB_TUSS_ID = 3208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3210 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.400' WHERE PROPTB_TUSS_ID = 3209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3211 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.470' WHERE PROPTB_TUSS_ID = 3210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3212 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.540' WHERE PROPTB_TUSS_ID = 3211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3213 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.610' WHERE PROPTB_TUSS_ID = 3212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3214 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.683' WHERE PROPTB_TUSS_ID = 3213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3215 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.753' WHERE PROPTB_TUSS_ID = 3214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3216 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.827' WHERE PROPTB_TUSS_ID = 3215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3217 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.897' WHERE PROPTB_TUSS_ID = 3216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3218 , CLOUD_SYNC_DATE = '2014-05-06 17:37:23.970' WHERE PROPTB_TUSS_ID = 3217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3219 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.040' WHERE PROPTB_TUSS_ID = 3218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3220 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.110' WHERE PROPTB_TUSS_ID = 3219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3221 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.180' WHERE PROPTB_TUSS_ID = 3220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3222 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.253' WHERE PROPTB_TUSS_ID = 3221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3223 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.323' WHERE PROPTB_TUSS_ID = 3222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3224 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.397' WHERE PROPTB_TUSS_ID = 3223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3225 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.467' WHERE PROPTB_TUSS_ID = 3224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3226 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.537' WHERE PROPTB_TUSS_ID = 3225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3227 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.607' WHERE PROPTB_TUSS_ID = 3226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3228 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.677' WHERE PROPTB_TUSS_ID = 3227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3229 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.753' WHERE PROPTB_TUSS_ID = 3228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3230 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.820' WHERE PROPTB_TUSS_ID = 3229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3231 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.900' WHERE PROPTB_TUSS_ID = 3230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3232 , CLOUD_SYNC_DATE = '2014-05-06 17:37:24.970' WHERE PROPTB_TUSS_ID = 3231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3233 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.043' WHERE PROPTB_TUSS_ID = 3232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3234 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.113' WHERE PROPTB_TUSS_ID = 3233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3235 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.187' WHERE PROPTB_TUSS_ID = 3234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3236 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.260' WHERE PROPTB_TUSS_ID = 3235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3237 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.330' WHERE PROPTB_TUSS_ID = 3236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3238 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.400' WHERE PROPTB_TUSS_ID = 3237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3239 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.473' WHERE PROPTB_TUSS_ID = 3238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3240 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.543' WHERE PROPTB_TUSS_ID = 3239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3241 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.617' WHERE PROPTB_TUSS_ID = 3240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3242 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.687' WHERE PROPTB_TUSS_ID = 3241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3243 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.760' WHERE PROPTB_TUSS_ID = 3242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3244 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.827' WHERE PROPTB_TUSS_ID = 3243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3245 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.900' WHERE PROPTB_TUSS_ID = 3244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3246 , CLOUD_SYNC_DATE = '2014-05-06 17:37:25.970' WHERE PROPTB_TUSS_ID = 3245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3247 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.043' WHERE PROPTB_TUSS_ID = 3246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3248 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.113' WHERE PROPTB_TUSS_ID = 3247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3249 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.187' WHERE PROPTB_TUSS_ID = 3248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3250 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.257' WHERE PROPTB_TUSS_ID = 3249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3251 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.327' WHERE PROPTB_TUSS_ID = 3250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3252 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.397' WHERE PROPTB_TUSS_ID = 3251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3253 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.470' WHERE PROPTB_TUSS_ID = 3252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3254 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.540' WHERE PROPTB_TUSS_ID = 3253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3255 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.613' WHERE PROPTB_TUSS_ID = 3254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3256 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.687' WHERE PROPTB_TUSS_ID = 3255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3257 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.763' WHERE PROPTB_TUSS_ID = 3256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3258 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.830' WHERE PROPTB_TUSS_ID = 3257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3259 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.907' WHERE PROPTB_TUSS_ID = 3258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3260 , CLOUD_SYNC_DATE = '2014-05-06 17:37:26.977' WHERE PROPTB_TUSS_ID = 3259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3261 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.053' WHERE PROPTB_TUSS_ID = 3260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3262 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.127' WHERE PROPTB_TUSS_ID = 3261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3263 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.200' WHERE PROPTB_TUSS_ID = 3262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3264 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.273' WHERE PROPTB_TUSS_ID = 3263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3265 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.343' WHERE PROPTB_TUSS_ID = 3264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3266 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.420' WHERE PROPTB_TUSS_ID = 3265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3267 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.490' WHERE PROPTB_TUSS_ID = 3266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3268 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.577' WHERE PROPTB_TUSS_ID = 3267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3269 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.657' WHERE PROPTB_TUSS_ID = 3268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3270 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.733' WHERE PROPTB_TUSS_ID = 3269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3271 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.803' WHERE PROPTB_TUSS_ID = 3270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3272 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.877' WHERE PROPTB_TUSS_ID = 3271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3273 , CLOUD_SYNC_DATE = '2014-05-06 17:37:27.947' WHERE PROPTB_TUSS_ID = 3272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3274 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.020' WHERE PROPTB_TUSS_ID = 3273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3275 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.090' WHERE PROPTB_TUSS_ID = 3274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3276 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.167' WHERE PROPTB_TUSS_ID = 3275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3277 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.237' WHERE PROPTB_TUSS_ID = 3276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3278 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.310' WHERE PROPTB_TUSS_ID = 3277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3279 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.387' WHERE PROPTB_TUSS_ID = 3278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3280 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.460' WHERE PROPTB_TUSS_ID = 3279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3281 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.537' WHERE PROPTB_TUSS_ID = 3280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3282 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.610' WHERE PROPTB_TUSS_ID = 3281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3283 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.697' WHERE PROPTB_TUSS_ID = 3282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3284 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.767' WHERE PROPTB_TUSS_ID = 3283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3285 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.847' WHERE PROPTB_TUSS_ID = 3284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3286 , CLOUD_SYNC_DATE = '2014-05-06 17:37:28.923' WHERE PROPTB_TUSS_ID = 3285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3287 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.000' WHERE PROPTB_TUSS_ID = 3286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3288 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.073' WHERE PROPTB_TUSS_ID = 3287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3289 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.147' WHERE PROPTB_TUSS_ID = 3288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3290 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.220' WHERE PROPTB_TUSS_ID = 3289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3291 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.300' WHERE PROPTB_TUSS_ID = 3290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3292 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.377' WHERE PROPTB_TUSS_ID = 3291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3293 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.457' WHERE PROPTB_TUSS_ID = 3292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3294 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.540' WHERE PROPTB_TUSS_ID = 3293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3295 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.617' WHERE PROPTB_TUSS_ID = 3294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3296 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.703' WHERE PROPTB_TUSS_ID = 3295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3297 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.777' WHERE PROPTB_TUSS_ID = 3296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3298 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.850' WHERE PROPTB_TUSS_ID = 3297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3299 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.920' WHERE PROPTB_TUSS_ID = 3298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3300 , CLOUD_SYNC_DATE = '2014-05-06 17:37:29.997' WHERE PROPTB_TUSS_ID = 3299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3301 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.077' WHERE PROPTB_TUSS_ID = 3300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3302 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.160' WHERE PROPTB_TUSS_ID = 3301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3303 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.243' WHERE PROPTB_TUSS_ID = 3302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3304 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.317' WHERE PROPTB_TUSS_ID = 3303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 2636 , CLOUD_SYNC_DATE = '2014-05-06 17:36:45.640' WHERE PROPTB_TUSS_ID = 3304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3306 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.480' WHERE PROPTB_TUSS_ID = 3305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3307 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.557' WHERE PROPTB_TUSS_ID = 3306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3308 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.630' WHERE PROPTB_TUSS_ID = 3307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3309 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.710' WHERE PROPTB_TUSS_ID = 3308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3310 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.790' WHERE PROPTB_TUSS_ID = 3309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3311 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.863' WHERE PROPTB_TUSS_ID = 3310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3312 , CLOUD_SYNC_DATE = '2014-05-06 17:37:30.937' WHERE PROPTB_TUSS_ID = 3311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3313 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.007' WHERE PROPTB_TUSS_ID = 3312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3314 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.083' WHERE PROPTB_TUSS_ID = 3313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3315 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.153' WHERE PROPTB_TUSS_ID = 3314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3316 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.227' WHERE PROPTB_TUSS_ID = 3315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3317 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.300' WHERE PROPTB_TUSS_ID = 3316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3318 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.377' WHERE PROPTB_TUSS_ID = 3317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3319 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.457' WHERE PROPTB_TUSS_ID = 3318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3320 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.547' WHERE PROPTB_TUSS_ID = 3319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3321 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.643' WHERE PROPTB_TUSS_ID = 3320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3322 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.733' WHERE PROPTB_TUSS_ID = 3321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3323 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.810' WHERE PROPTB_TUSS_ID = 3322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3324 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.883' WHERE PROPTB_TUSS_ID = 3323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3325 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.953' WHERE PROPTB_TUSS_ID = 3324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3326 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.027' WHERE PROPTB_TUSS_ID = 3325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3327 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.103' WHERE PROPTB_TUSS_ID = 3326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3328 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.173' WHERE PROPTB_TUSS_ID = 3327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3329 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.330' WHERE PROPTB_TUSS_ID = 3328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3330 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.450' WHERE PROPTB_TUSS_ID = 3329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3331 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.540' WHERE PROPTB_TUSS_ID = 3330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3332 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.633' WHERE PROPTB_TUSS_ID = 3331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3333 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.720' WHERE PROPTB_TUSS_ID = 3332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3334 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.827' WHERE PROPTB_TUSS_ID = 3333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3335 , CLOUD_SYNC_DATE = '2014-05-06 17:37:32.923' WHERE PROPTB_TUSS_ID = 3334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3336 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.010' WHERE PROPTB_TUSS_ID = 3335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3337 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.087' WHERE PROPTB_TUSS_ID = 3336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3338 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.163' WHERE PROPTB_TUSS_ID = 3337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3339 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.243' WHERE PROPTB_TUSS_ID = 3338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3340 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.317' WHERE PROPTB_TUSS_ID = 3339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3341 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.387' WHERE PROPTB_TUSS_ID = 3340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3342 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.463' WHERE PROPTB_TUSS_ID = 3341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3343 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.537' WHERE PROPTB_TUSS_ID = 3342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3344 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.613' WHERE PROPTB_TUSS_ID = 3343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3345 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.693' WHERE PROPTB_TUSS_ID = 3344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3346 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.767' WHERE PROPTB_TUSS_ID = 3345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3347 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.847' WHERE PROPTB_TUSS_ID = 3346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3348 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.917' WHERE PROPTB_TUSS_ID = 3347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3349 , CLOUD_SYNC_DATE = '2014-05-06 17:37:33.993' WHERE PROPTB_TUSS_ID = 3348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3350 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.063' WHERE PROPTB_TUSS_ID = 3349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3351 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.137' WHERE PROPTB_TUSS_ID = 3350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3352 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.210' WHERE PROPTB_TUSS_ID = 3351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3353 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.283' WHERE PROPTB_TUSS_ID = 3352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3354 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.357' WHERE PROPTB_TUSS_ID = 3353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3355 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.430' WHERE PROPTB_TUSS_ID = 3354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3356 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.513' WHERE PROPTB_TUSS_ID = 3355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3357 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.597' WHERE PROPTB_TUSS_ID = 3356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3358 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.673' WHERE PROPTB_TUSS_ID = 3357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3359 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.747' WHERE PROPTB_TUSS_ID = 3358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3360 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.823' WHERE PROPTB_TUSS_ID = 3359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3361 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.897' WHERE PROPTB_TUSS_ID = 3360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3362 , CLOUD_SYNC_DATE = '2014-05-06 17:37:34.970' WHERE PROPTB_TUSS_ID = 3361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3363 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.043' WHERE PROPTB_TUSS_ID = 3362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3364 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.117' WHERE PROPTB_TUSS_ID = 3363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3365 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.197' WHERE PROPTB_TUSS_ID = 3364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3366 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.270' WHERE PROPTB_TUSS_ID = 3365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3367 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.347' WHERE PROPTB_TUSS_ID = 3366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3368 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.420' WHERE PROPTB_TUSS_ID = 3367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3369 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.490' WHERE PROPTB_TUSS_ID = 3368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3370 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.567' WHERE PROPTB_TUSS_ID = 3369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3371 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.643' WHERE PROPTB_TUSS_ID = 3370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3372 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.737' WHERE PROPTB_TUSS_ID = 3371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3373 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.857' WHERE PROPTB_TUSS_ID = 3372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3374 , CLOUD_SYNC_DATE = '2014-05-06 17:37:35.960' WHERE PROPTB_TUSS_ID = 3373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3375 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.077' WHERE PROPTB_TUSS_ID = 3374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3376 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.183' WHERE PROPTB_TUSS_ID = 3375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3377 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.273' WHERE PROPTB_TUSS_ID = 3376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3378 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.410' WHERE PROPTB_TUSS_ID = 3377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3379 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.500' WHERE PROPTB_TUSS_ID = 3378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3380 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.573' WHERE PROPTB_TUSS_ID = 3379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3381 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.650' WHERE PROPTB_TUSS_ID = 3380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3382 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.733' WHERE PROPTB_TUSS_ID = 3381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3383 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.810' WHERE PROPTB_TUSS_ID = 3382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3384 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.910' WHERE PROPTB_TUSS_ID = 3383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3385 , CLOUD_SYNC_DATE = '2014-05-06 17:37:36.990' WHERE PROPTB_TUSS_ID = 3384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3386 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.067' WHERE PROPTB_TUSS_ID = 3385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3387 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.190' WHERE PROPTB_TUSS_ID = 3386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3388 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.287' WHERE PROPTB_TUSS_ID = 3387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3389 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.367' WHERE PROPTB_TUSS_ID = 3388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3390 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.447' WHERE PROPTB_TUSS_ID = 3389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3391 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.517' WHERE PROPTB_TUSS_ID = 3390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3392 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.593' WHERE PROPTB_TUSS_ID = 3391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3393 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.677' WHERE PROPTB_TUSS_ID = 3392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3394 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.750' WHERE PROPTB_TUSS_ID = 3393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3395 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.830' WHERE PROPTB_TUSS_ID = 3394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3396 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.913' WHERE PROPTB_TUSS_ID = 3395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3397 , CLOUD_SYNC_DATE = '2014-05-06 17:37:37.990' WHERE PROPTB_TUSS_ID = 3396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3398 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.067' WHERE PROPTB_TUSS_ID = 3397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3399 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.143' WHERE PROPTB_TUSS_ID = 3398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3400 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.217' WHERE PROPTB_TUSS_ID = 3399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3401 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.293' WHERE PROPTB_TUSS_ID = 3400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3402 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.370' WHERE PROPTB_TUSS_ID = 3401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3403 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.440' WHERE PROPTB_TUSS_ID = 3402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3404 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.517' WHERE PROPTB_TUSS_ID = 3403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3405 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.593' WHERE PROPTB_TUSS_ID = 3404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3406 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.670' WHERE PROPTB_TUSS_ID = 3405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3407 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.747' WHERE PROPTB_TUSS_ID = 3406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3408 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.823' WHERE PROPTB_TUSS_ID = 3407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3409 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.903' WHERE PROPTB_TUSS_ID = 3408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3410 , CLOUD_SYNC_DATE = '2014-05-06 17:37:38.977' WHERE PROPTB_TUSS_ID = 3409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3411 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.063' WHERE PROPTB_TUSS_ID = 3410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3412 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.143' WHERE PROPTB_TUSS_ID = 3411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3413 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.220' WHERE PROPTB_TUSS_ID = 3412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3414 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.297' WHERE PROPTB_TUSS_ID = 3413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3415 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.373' WHERE PROPTB_TUSS_ID = 3414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3416 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.457' WHERE PROPTB_TUSS_ID = 3415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3417 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.540' WHERE PROPTB_TUSS_ID = 3416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3418 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.620' WHERE PROPTB_TUSS_ID = 3417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3419 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.707' WHERE PROPTB_TUSS_ID = 3418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3420 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.783' WHERE PROPTB_TUSS_ID = 3419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3421 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.860' WHERE PROPTB_TUSS_ID = 3420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3422 , CLOUD_SYNC_DATE = '2014-05-06 17:37:39.933' WHERE PROPTB_TUSS_ID = 3421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3423 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.027' WHERE PROPTB_TUSS_ID = 3422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3424 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.103' WHERE PROPTB_TUSS_ID = 3423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3425 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.183' WHERE PROPTB_TUSS_ID = 3424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3426 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.260' WHERE PROPTB_TUSS_ID = 3425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3427 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.337' WHERE PROPTB_TUSS_ID = 3426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3428 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.410' WHERE PROPTB_TUSS_ID = 3427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3429 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.490' WHERE PROPTB_TUSS_ID = 3428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3430 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.567' WHERE PROPTB_TUSS_ID = 3429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3431 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.640' WHERE PROPTB_TUSS_ID = 3430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3432 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.720' WHERE PROPTB_TUSS_ID = 3431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3433 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.797' WHERE PROPTB_TUSS_ID = 3432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3434 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.883' WHERE PROPTB_TUSS_ID = 3433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3435 , CLOUD_SYNC_DATE = '2014-05-06 17:37:40.967' WHERE PROPTB_TUSS_ID = 3434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3436 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.053' WHERE PROPTB_TUSS_ID = 3435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3437 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.143' WHERE PROPTB_TUSS_ID = 3436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3438 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.220' WHERE PROPTB_TUSS_ID = 3437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3439 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.300' WHERE PROPTB_TUSS_ID = 3438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3440 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.380' WHERE PROPTB_TUSS_ID = 3439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3441 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.460' WHERE PROPTB_TUSS_ID = 3440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3442 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.537' WHERE PROPTB_TUSS_ID = 3441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3443 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.613' WHERE PROPTB_TUSS_ID = 3442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3444 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.693' WHERE PROPTB_TUSS_ID = 3443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3445 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.770' WHERE PROPTB_TUSS_ID = 3444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3446 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.847' WHERE PROPTB_TUSS_ID = 3445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3447 , CLOUD_SYNC_DATE = '2014-05-06 17:37:41.927' WHERE PROPTB_TUSS_ID = 3446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3448 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.013' WHERE PROPTB_TUSS_ID = 3447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3449 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.093' WHERE PROPTB_TUSS_ID = 3448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3450 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.170' WHERE PROPTB_TUSS_ID = 3449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3451 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.247' WHERE PROPTB_TUSS_ID = 3450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3452 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.323' WHERE PROPTB_TUSS_ID = 3451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3453 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.407' WHERE PROPTB_TUSS_ID = 3452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3454 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.497' WHERE PROPTB_TUSS_ID = 3453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3455 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.580' WHERE PROPTB_TUSS_ID = 3454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3456 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.657' WHERE PROPTB_TUSS_ID = 3455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3457 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.740' WHERE PROPTB_TUSS_ID = 3456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3458 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.827' WHERE PROPTB_TUSS_ID = 3457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3459 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.907' WHERE PROPTB_TUSS_ID = 3458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3460 , CLOUD_SYNC_DATE = '2014-05-06 17:37:42.980' WHERE PROPTB_TUSS_ID = 3459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3461 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.067' WHERE PROPTB_TUSS_ID = 3460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3462 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.143' WHERE PROPTB_TUSS_ID = 3461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3463 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.220' WHERE PROPTB_TUSS_ID = 3462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3464 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.300' WHERE PROPTB_TUSS_ID = 3463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3465 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.383' WHERE PROPTB_TUSS_ID = 3464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3466 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.463' WHERE PROPTB_TUSS_ID = 3465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3467 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.547' WHERE PROPTB_TUSS_ID = 3466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3468 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.627' WHERE PROPTB_TUSS_ID = 3467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3469 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.710' WHERE PROPTB_TUSS_ID = 3468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3470 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.790' WHERE PROPTB_TUSS_ID = 3469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3471 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.867' WHERE PROPTB_TUSS_ID = 3470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3472 , CLOUD_SYNC_DATE = '2014-05-06 17:37:43.947' WHERE PROPTB_TUSS_ID = 3471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3473 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.027' WHERE PROPTB_TUSS_ID = 3472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3474 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.103' WHERE PROPTB_TUSS_ID = 3473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3475 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.180' WHERE PROPTB_TUSS_ID = 3474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3476 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.260' WHERE PROPTB_TUSS_ID = 3475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3477 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.340' WHERE PROPTB_TUSS_ID = 3476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3478 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.413' WHERE PROPTB_TUSS_ID = 3477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3479 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.490' WHERE PROPTB_TUSS_ID = 3478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3480 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.570' WHERE PROPTB_TUSS_ID = 3479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3481 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.653' WHERE PROPTB_TUSS_ID = 3480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3482 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.743' WHERE PROPTB_TUSS_ID = 3481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3483 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.823' WHERE PROPTB_TUSS_ID = 3482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3484 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.910' WHERE PROPTB_TUSS_ID = 3483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3485 , CLOUD_SYNC_DATE = '2014-05-06 17:37:44.997' WHERE PROPTB_TUSS_ID = 3484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3486 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.087' WHERE PROPTB_TUSS_ID = 3485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3487 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.167' WHERE PROPTB_TUSS_ID = 3486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3488 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.247' WHERE PROPTB_TUSS_ID = 3487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3489 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.333' WHERE PROPTB_TUSS_ID = 3488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3490 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.417' WHERE PROPTB_TUSS_ID = 3489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3491 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.493' WHERE PROPTB_TUSS_ID = 3490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3492 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.573' WHERE PROPTB_TUSS_ID = 3491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3493 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.650' WHERE PROPTB_TUSS_ID = 3492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3494 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.733' WHERE PROPTB_TUSS_ID = 3493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3495 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.807' WHERE PROPTB_TUSS_ID = 3494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3496 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.887' WHERE PROPTB_TUSS_ID = 3495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3497 , CLOUD_SYNC_DATE = '2014-05-06 17:37:45.963' WHERE PROPTB_TUSS_ID = 3496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3498 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.043' WHERE PROPTB_TUSS_ID = 3497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3499 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.117' WHERE PROPTB_TUSS_ID = 3498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3500 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.193' WHERE PROPTB_TUSS_ID = 3499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3501 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.270' WHERE PROPTB_TUSS_ID = 3500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3502 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.350' WHERE PROPTB_TUSS_ID = 3501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3503 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.430' WHERE PROPTB_TUSS_ID = 3502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3504 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.507' WHERE PROPTB_TUSS_ID = 3503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3505 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.587' WHERE PROPTB_TUSS_ID = 3504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3506 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.667' WHERE PROPTB_TUSS_ID = 3505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3507 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.747' WHERE PROPTB_TUSS_ID = 3506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3508 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.830' WHERE PROPTB_TUSS_ID = 3507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3509 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.910' WHERE PROPTB_TUSS_ID = 3508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3510 , CLOUD_SYNC_DATE = '2014-05-06 17:37:46.990' WHERE PROPTB_TUSS_ID = 3509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3511 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.073' WHERE PROPTB_TUSS_ID = 3510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3512 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.157' WHERE PROPTB_TUSS_ID = 3511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3513 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.237' WHERE PROPTB_TUSS_ID = 3512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3514 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.317' WHERE PROPTB_TUSS_ID = 3513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3515 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.393' WHERE PROPTB_TUSS_ID = 3514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3516 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.473' WHERE PROPTB_TUSS_ID = 3515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3517 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.553' WHERE PROPTB_TUSS_ID = 3516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3518 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.630' WHERE PROPTB_TUSS_ID = 3517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3519 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.710' WHERE PROPTB_TUSS_ID = 3518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3520 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.790' WHERE PROPTB_TUSS_ID = 3519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3521 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.873' WHERE PROPTB_TUSS_ID = 3520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3522 , CLOUD_SYNC_DATE = '2014-05-06 17:37:47.953' WHERE PROPTB_TUSS_ID = 3521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3523 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.030' WHERE PROPTB_TUSS_ID = 3522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3524 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.117' WHERE PROPTB_TUSS_ID = 3523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3525 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.197' WHERE PROPTB_TUSS_ID = 3524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3526 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.273' WHERE PROPTB_TUSS_ID = 3525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3527 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.350' WHERE PROPTB_TUSS_ID = 3526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3528 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.427' WHERE PROPTB_TUSS_ID = 3527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3529 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.507' WHERE PROPTB_TUSS_ID = 3528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3530 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.593' WHERE PROPTB_TUSS_ID = 3529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3531 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.687' WHERE PROPTB_TUSS_ID = 3530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3532 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.767' WHERE PROPTB_TUSS_ID = 3531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3533 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.847' WHERE PROPTB_TUSS_ID = 3532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3534 , CLOUD_SYNC_DATE = '2014-05-06 17:37:48.923' WHERE PROPTB_TUSS_ID = 3533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3535 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.003' WHERE PROPTB_TUSS_ID = 3534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3536 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.083' WHERE PROPTB_TUSS_ID = 3535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3537 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.167' WHERE PROPTB_TUSS_ID = 3536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3538 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.253' WHERE PROPTB_TUSS_ID = 3537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3539 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.337' WHERE PROPTB_TUSS_ID = 3538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3540 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.427' WHERE PROPTB_TUSS_ID = 3539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3541 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.507' WHERE PROPTB_TUSS_ID = 3540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3542 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.590' WHERE PROPTB_TUSS_ID = 3541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3543 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.670' WHERE PROPTB_TUSS_ID = 3542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3544 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.767' WHERE PROPTB_TUSS_ID = 3543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3545 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.850' WHERE PROPTB_TUSS_ID = 3544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3546 , CLOUD_SYNC_DATE = '2014-05-06 17:37:49.927' WHERE PROPTB_TUSS_ID = 3545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3547 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.007' WHERE PROPTB_TUSS_ID = 3546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3548 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.087' WHERE PROPTB_TUSS_ID = 3547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3549 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.163' WHERE PROPTB_TUSS_ID = 3548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3550 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.237' WHERE PROPTB_TUSS_ID = 3549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3551 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.317' WHERE PROPTB_TUSS_ID = 3550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3552 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.393' WHERE PROPTB_TUSS_ID = 3551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3553 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.473' WHERE PROPTB_TUSS_ID = 3552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3554 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.557' WHERE PROPTB_TUSS_ID = 3553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3555 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.637' WHERE PROPTB_TUSS_ID = 3554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3556 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.717' WHERE PROPTB_TUSS_ID = 3555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3557 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.797' WHERE PROPTB_TUSS_ID = 3556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3558 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.890' WHERE PROPTB_TUSS_ID = 3557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3559 , CLOUD_SYNC_DATE = '2014-05-06 17:37:50.970' WHERE PROPTB_TUSS_ID = 3558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3560 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.047' WHERE PROPTB_TUSS_ID = 3559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3561 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.130' WHERE PROPTB_TUSS_ID = 3560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3562 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.207' WHERE PROPTB_TUSS_ID = 3561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3563 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.283' WHERE PROPTB_TUSS_ID = 3562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3564 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.363' WHERE PROPTB_TUSS_ID = 3563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3565 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.443' WHERE PROPTB_TUSS_ID = 3564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3566 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.520' WHERE PROPTB_TUSS_ID = 3565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3567 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.600' WHERE PROPTB_TUSS_ID = 3566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3568 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.687' WHERE PROPTB_TUSS_ID = 3567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3569 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.767' WHERE PROPTB_TUSS_ID = 3568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3570 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.847' WHERE PROPTB_TUSS_ID = 3569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3571 , CLOUD_SYNC_DATE = '2014-05-06 17:37:51.923' WHERE PROPTB_TUSS_ID = 3570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3572 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.000' WHERE PROPTB_TUSS_ID = 3571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3573 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.083' WHERE PROPTB_TUSS_ID = 3572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3574 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.167' WHERE PROPTB_TUSS_ID = 3573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3575 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.253' WHERE PROPTB_TUSS_ID = 3574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3576 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.340' WHERE PROPTB_TUSS_ID = 3575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3577 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.420' WHERE PROPTB_TUSS_ID = 3576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3578 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.507' WHERE PROPTB_TUSS_ID = 3577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3579 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.590' WHERE PROPTB_TUSS_ID = 3578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3580 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.673' WHERE PROPTB_TUSS_ID = 3579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3581 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.753' WHERE PROPTB_TUSS_ID = 3580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3582 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.830' WHERE PROPTB_TUSS_ID = 3581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3583 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.910' WHERE PROPTB_TUSS_ID = 3582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3584 , CLOUD_SYNC_DATE = '2014-05-06 17:37:52.990' WHERE PROPTB_TUSS_ID = 3583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3585 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.070' WHERE PROPTB_TUSS_ID = 3584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3586 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.150' WHERE PROPTB_TUSS_ID = 3585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3587 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.227' WHERE PROPTB_TUSS_ID = 3586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3588 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.310' WHERE PROPTB_TUSS_ID = 3587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3589 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.393' WHERE PROPTB_TUSS_ID = 3588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3590 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.477' WHERE PROPTB_TUSS_ID = 3589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3591 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.560' WHERE PROPTB_TUSS_ID = 3590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3592 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.643' WHERE PROPTB_TUSS_ID = 3591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3593 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.733' WHERE PROPTB_TUSS_ID = 3592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3594 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.813' WHERE PROPTB_TUSS_ID = 3593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3595 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.890' WHERE PROPTB_TUSS_ID = 3594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3596 , CLOUD_SYNC_DATE = '2014-05-06 17:37:53.973' WHERE PROPTB_TUSS_ID = 3595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3597 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.067' WHERE PROPTB_TUSS_ID = 3596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3598 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.150' WHERE PROPTB_TUSS_ID = 3597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3599 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.233' WHERE PROPTB_TUSS_ID = 3598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3600 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.313' WHERE PROPTB_TUSS_ID = 3599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3601 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.400' WHERE PROPTB_TUSS_ID = 3600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3602 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.483' WHERE PROPTB_TUSS_ID = 3601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3603 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.567' WHERE PROPTB_TUSS_ID = 3602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3604 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.653' WHERE PROPTB_TUSS_ID = 3603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3605 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.737' WHERE PROPTB_TUSS_ID = 3604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3606 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.817' WHERE PROPTB_TUSS_ID = 3605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3607 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.897' WHERE PROPTB_TUSS_ID = 3606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3608 , CLOUD_SYNC_DATE = '2014-05-06 17:37:54.980' WHERE PROPTB_TUSS_ID = 3607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3609 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.063' WHERE PROPTB_TUSS_ID = 3608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3610 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.147' WHERE PROPTB_TUSS_ID = 3609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3611 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.223' WHERE PROPTB_TUSS_ID = 3610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3612 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.303' WHERE PROPTB_TUSS_ID = 3611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3613 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.383' WHERE PROPTB_TUSS_ID = 3612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3614 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.460' WHERE PROPTB_TUSS_ID = 3613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3615 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.540' WHERE PROPTB_TUSS_ID = 3614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3616 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.620' WHERE PROPTB_TUSS_ID = 3615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3617 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.703' WHERE PROPTB_TUSS_ID = 3616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3618 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.787' WHERE PROPTB_TUSS_ID = 3617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3619 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.870' WHERE PROPTB_TUSS_ID = 3618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3620 , CLOUD_SYNC_DATE = '2014-05-06 17:37:55.950' WHERE PROPTB_TUSS_ID = 3619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3621 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.033' WHERE PROPTB_TUSS_ID = 3620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3622 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.117' WHERE PROPTB_TUSS_ID = 3621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3623 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.197' WHERE PROPTB_TUSS_ID = 3622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3624 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.277' WHERE PROPTB_TUSS_ID = 3623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3625 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.353' WHERE PROPTB_TUSS_ID = 3624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3626 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.433' WHERE PROPTB_TUSS_ID = 3625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3627 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.513' WHERE PROPTB_TUSS_ID = 3626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3628 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.597' WHERE PROPTB_TUSS_ID = 3627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3629 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.680' WHERE PROPTB_TUSS_ID = 3628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3630 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.760' WHERE PROPTB_TUSS_ID = 3629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3631 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.843' WHERE PROPTB_TUSS_ID = 3630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3632 , CLOUD_SYNC_DATE = '2014-05-06 17:37:56.923' WHERE PROPTB_TUSS_ID = 3631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3633 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.003' WHERE PROPTB_TUSS_ID = 3632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3634 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.087' WHERE PROPTB_TUSS_ID = 3633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3635 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.167' WHERE PROPTB_TUSS_ID = 3634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3636 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.247' WHERE PROPTB_TUSS_ID = 3635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3637 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.327' WHERE PROPTB_TUSS_ID = 3636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3638 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.410' WHERE PROPTB_TUSS_ID = 3637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3639 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.493' WHERE PROPTB_TUSS_ID = 3638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3640 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.573' WHERE PROPTB_TUSS_ID = 3639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3641 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.657' WHERE PROPTB_TUSS_ID = 3640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3642 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.747' WHERE PROPTB_TUSS_ID = 3641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3643 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.827' WHERE PROPTB_TUSS_ID = 3642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3644 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.910' WHERE PROPTB_TUSS_ID = 3643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3645 , CLOUD_SYNC_DATE = '2014-05-06 17:37:57.993' WHERE PROPTB_TUSS_ID = 3644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3646 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.077' WHERE PROPTB_TUSS_ID = 3645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3647 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.157' WHERE PROPTB_TUSS_ID = 3646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3648 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.243' WHERE PROPTB_TUSS_ID = 3647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3649 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.327' WHERE PROPTB_TUSS_ID = 3648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3650 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.410' WHERE PROPTB_TUSS_ID = 3649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3651 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.490' WHERE PROPTB_TUSS_ID = 3650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3652 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.570' WHERE PROPTB_TUSS_ID = 3651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3653 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.657' WHERE PROPTB_TUSS_ID = 3652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3654 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.747' WHERE PROPTB_TUSS_ID = 3653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3655 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.830' WHERE PROPTB_TUSS_ID = 3654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3656 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.910' WHERE PROPTB_TUSS_ID = 3655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3657 , CLOUD_SYNC_DATE = '2014-05-06 17:37:58.990' WHERE PROPTB_TUSS_ID = 3656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3658 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.073' WHERE PROPTB_TUSS_ID = 3657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3659 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.157' WHERE PROPTB_TUSS_ID = 3658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3660 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.243' WHERE PROPTB_TUSS_ID = 3659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3661 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.323' WHERE PROPTB_TUSS_ID = 3660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3662 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.403' WHERE PROPTB_TUSS_ID = 3661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3663 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.487' WHERE PROPTB_TUSS_ID = 3662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3664 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.567' WHERE PROPTB_TUSS_ID = 3663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3665 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.653' WHERE PROPTB_TUSS_ID = 3664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3666 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.743' WHERE PROPTB_TUSS_ID = 3665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3667 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.827' WHERE PROPTB_TUSS_ID = 3666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3668 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.910' WHERE PROPTB_TUSS_ID = 3667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3669 , CLOUD_SYNC_DATE = '2014-05-06 17:37:59.993' WHERE PROPTB_TUSS_ID = 3668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3670 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.077' WHERE PROPTB_TUSS_ID = 3669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3671 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.160' WHERE PROPTB_TUSS_ID = 3670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3672 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.240' WHERE PROPTB_TUSS_ID = 3671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3673 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.323' WHERE PROPTB_TUSS_ID = 3672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3674 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.403' WHERE PROPTB_TUSS_ID = 3673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3675 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.523' WHERE PROPTB_TUSS_ID = 3674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3676 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.607' WHERE PROPTB_TUSS_ID = 3675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3677 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.693' WHERE PROPTB_TUSS_ID = 3676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3678 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.773' WHERE PROPTB_TUSS_ID = 3677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3679 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.853' WHERE PROPTB_TUSS_ID = 3678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3680 , CLOUD_SYNC_DATE = '2014-05-06 17:38:00.933' WHERE PROPTB_TUSS_ID = 3679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3681 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.017' WHERE PROPTB_TUSS_ID = 3680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3682 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.097' WHERE PROPTB_TUSS_ID = 3681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3683 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.180' WHERE PROPTB_TUSS_ID = 3682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3684 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.263' WHERE PROPTB_TUSS_ID = 3683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3685 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.343' WHERE PROPTB_TUSS_ID = 3684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3686 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.427' WHERE PROPTB_TUSS_ID = 3685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3687 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.510' WHERE PROPTB_TUSS_ID = 3686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3688 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.597' WHERE PROPTB_TUSS_ID = 3687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3689 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.687' WHERE PROPTB_TUSS_ID = 3688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3690 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.767' WHERE PROPTB_TUSS_ID = 3689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3691 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.853' WHERE PROPTB_TUSS_ID = 3690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3692 , CLOUD_SYNC_DATE = '2014-05-06 17:38:01.937' WHERE PROPTB_TUSS_ID = 3691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3693 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.020' WHERE PROPTB_TUSS_ID = 3692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3694 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.100' WHERE PROPTB_TUSS_ID = 3693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3695 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.183' WHERE PROPTB_TUSS_ID = 3694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3696 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.263' WHERE PROPTB_TUSS_ID = 3695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3697 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.343' WHERE PROPTB_TUSS_ID = 3696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3698 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.427' WHERE PROPTB_TUSS_ID = 3697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3699 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.510' WHERE PROPTB_TUSS_ID = 3698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3700 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.603' WHERE PROPTB_TUSS_ID = 3699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3701 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.687' WHERE PROPTB_TUSS_ID = 3700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3702 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.767' WHERE PROPTB_TUSS_ID = 3701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3703 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.850' WHERE PROPTB_TUSS_ID = 3702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3704 , CLOUD_SYNC_DATE = '2014-05-06 17:38:02.930' WHERE PROPTB_TUSS_ID = 3703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3705 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.013' WHERE PROPTB_TUSS_ID = 3704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3706 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.093' WHERE PROPTB_TUSS_ID = 3705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3707 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.177' WHERE PROPTB_TUSS_ID = 3706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3708 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.257' WHERE PROPTB_TUSS_ID = 3707
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3709 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.340' WHERE PROPTB_TUSS_ID = 3708
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3710 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.420' WHERE PROPTB_TUSS_ID = 3709
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3711 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.510' WHERE PROPTB_TUSS_ID = 3710
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3712 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.593' WHERE PROPTB_TUSS_ID = 3711
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3713 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.680' WHERE PROPTB_TUSS_ID = 3712
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3714 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.763' WHERE PROPTB_TUSS_ID = 3713
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3715 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.847' WHERE PROPTB_TUSS_ID = 3714
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3716 , CLOUD_SYNC_DATE = '2014-05-06 17:38:03.930' WHERE PROPTB_TUSS_ID = 3715
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3717 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.013' WHERE PROPTB_TUSS_ID = 3716
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3718 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.097' WHERE PROPTB_TUSS_ID = 3717
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3719 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.180' WHERE PROPTB_TUSS_ID = 3718
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3720 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.263' WHERE PROPTB_TUSS_ID = 3719
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3721 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.347' WHERE PROPTB_TUSS_ID = 3720
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3722 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.427' WHERE PROPTB_TUSS_ID = 3721
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3723 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.510' WHERE PROPTB_TUSS_ID = 3722
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3724 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.593' WHERE PROPTB_TUSS_ID = 3723
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3725 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.677' WHERE PROPTB_TUSS_ID = 3724
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3726 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.763' WHERE PROPTB_TUSS_ID = 3725
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3727 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.847' WHERE PROPTB_TUSS_ID = 3726
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3728 , CLOUD_SYNC_DATE = '2014-05-06 17:38:04.927' WHERE PROPTB_TUSS_ID = 3727
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3729 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.010' WHERE PROPTB_TUSS_ID = 3728
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3730 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.093' WHERE PROPTB_TUSS_ID = 3729
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3731 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.180' WHERE PROPTB_TUSS_ID = 3730
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3732 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.260' WHERE PROPTB_TUSS_ID = 3731
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3733 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.343' WHERE PROPTB_TUSS_ID = 3732
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3734 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.427' WHERE PROPTB_TUSS_ID = 3733
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3735 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.510' WHERE PROPTB_TUSS_ID = 3734
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3736 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.593' WHERE PROPTB_TUSS_ID = 3735
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3737 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.677' WHERE PROPTB_TUSS_ID = 3736
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3738 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.760' WHERE PROPTB_TUSS_ID = 3737
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3739 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.840' WHERE PROPTB_TUSS_ID = 3738
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3740 , CLOUD_SYNC_DATE = '2014-05-06 17:38:05.923' WHERE PROPTB_TUSS_ID = 3739
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3741 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.007' WHERE PROPTB_TUSS_ID = 3740
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3742 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.087' WHERE PROPTB_TUSS_ID = 3741
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3743 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.173' WHERE PROPTB_TUSS_ID = 3742
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3744 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.253' WHERE PROPTB_TUSS_ID = 3743
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3745 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.337' WHERE PROPTB_TUSS_ID = 3744
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3746 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.417' WHERE PROPTB_TUSS_ID = 3745
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3747 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.500' WHERE PROPTB_TUSS_ID = 3746
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3748 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.583' WHERE PROPTB_TUSS_ID = 3747
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3749 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.693' WHERE PROPTB_TUSS_ID = 3748
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3750 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.777' WHERE PROPTB_TUSS_ID = 3749
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3751 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.857' WHERE PROPTB_TUSS_ID = 3750
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3752 , CLOUD_SYNC_DATE = '2014-05-06 17:38:06.943' WHERE PROPTB_TUSS_ID = 3751
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3753 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.023' WHERE PROPTB_TUSS_ID = 3752
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3754 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.127' WHERE PROPTB_TUSS_ID = 3753
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3755 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.217' WHERE PROPTB_TUSS_ID = 3754
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3756 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.303' WHERE PROPTB_TUSS_ID = 3755
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3757 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.390' WHERE PROPTB_TUSS_ID = 3756
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3758 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.473' WHERE PROPTB_TUSS_ID = 3757
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3759 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.560' WHERE PROPTB_TUSS_ID = 3758
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3760 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.647' WHERE PROPTB_TUSS_ID = 3759
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3761 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.730' WHERE PROPTB_TUSS_ID = 3760
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3762 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.813' WHERE PROPTB_TUSS_ID = 3761
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3763 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.897' WHERE PROPTB_TUSS_ID = 3762
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3764 , CLOUD_SYNC_DATE = '2014-05-06 17:38:07.990' WHERE PROPTB_TUSS_ID = 3763
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3765 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.073' WHERE PROPTB_TUSS_ID = 3764
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3766 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.160' WHERE PROPTB_TUSS_ID = 3765
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3767 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.240' WHERE PROPTB_TUSS_ID = 3766
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3768 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.323' WHERE PROPTB_TUSS_ID = 3767
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3769 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.407' WHERE PROPTB_TUSS_ID = 3768
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3770 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.487' WHERE PROPTB_TUSS_ID = 3769
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3771 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.570' WHERE PROPTB_TUSS_ID = 3770
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3772 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.657' WHERE PROPTB_TUSS_ID = 3771
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3773 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.740' WHERE PROPTB_TUSS_ID = 3772
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3774 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.827' WHERE PROPTB_TUSS_ID = 3773
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3775 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.913' WHERE PROPTB_TUSS_ID = 3774
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3776 , CLOUD_SYNC_DATE = '2014-05-06 17:38:08.997' WHERE PROPTB_TUSS_ID = 3775
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3777 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.080' WHERE PROPTB_TUSS_ID = 3776
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3778 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.163' WHERE PROPTB_TUSS_ID = 3777
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3779 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.247' WHERE PROPTB_TUSS_ID = 3778
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3780 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.333' WHERE PROPTB_TUSS_ID = 3779
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3781 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.413' WHERE PROPTB_TUSS_ID = 3780
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3782 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.497' WHERE PROPTB_TUSS_ID = 3781
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3783 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.583' WHERE PROPTB_TUSS_ID = 3782
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3784 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.670' WHERE PROPTB_TUSS_ID = 3783
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3785 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.757' WHERE PROPTB_TUSS_ID = 3784
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3786 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.843' WHERE PROPTB_TUSS_ID = 3785
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3787 , CLOUD_SYNC_DATE = '2014-05-06 17:38:09.927' WHERE PROPTB_TUSS_ID = 3786
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3788 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.010' WHERE PROPTB_TUSS_ID = 3787
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3789 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.093' WHERE PROPTB_TUSS_ID = 3788
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3790 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.180' WHERE PROPTB_TUSS_ID = 3789
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3791 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.267' WHERE PROPTB_TUSS_ID = 3790
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3792 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.347' WHERE PROPTB_TUSS_ID = 3791
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3793 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.430' WHERE PROPTB_TUSS_ID = 3792
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3794 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.517' WHERE PROPTB_TUSS_ID = 3793
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3795 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.600' WHERE PROPTB_TUSS_ID = 3794
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3796 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.687' WHERE PROPTB_TUSS_ID = 3795
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3797 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.777' WHERE PROPTB_TUSS_ID = 3796
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3798 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.860' WHERE PROPTB_TUSS_ID = 3797
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3799 , CLOUD_SYNC_DATE = '2014-05-06 17:38:10.943' WHERE PROPTB_TUSS_ID = 3798
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3800 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.027' WHERE PROPTB_TUSS_ID = 3799
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3801 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.110' WHERE PROPTB_TUSS_ID = 3800
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3802 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.197' WHERE PROPTB_TUSS_ID = 3801
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3803 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.280' WHERE PROPTB_TUSS_ID = 3802
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3804 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.367' WHERE PROPTB_TUSS_ID = 3803
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3805 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.450' WHERE PROPTB_TUSS_ID = 3804
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3806 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.533' WHERE PROPTB_TUSS_ID = 3805
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3807 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.617' WHERE PROPTB_TUSS_ID = 3806
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3808 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.707' WHERE PROPTB_TUSS_ID = 3807
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3809 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.790' WHERE PROPTB_TUSS_ID = 3808
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3810 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.877' WHERE PROPTB_TUSS_ID = 3809
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3811 , CLOUD_SYNC_DATE = '2014-05-06 17:38:11.960' WHERE PROPTB_TUSS_ID = 3810
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3812 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.043' WHERE PROPTB_TUSS_ID = 3811
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3813 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.127' WHERE PROPTB_TUSS_ID = 3812
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3814 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.213' WHERE PROPTB_TUSS_ID = 3813
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3815 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.297' WHERE PROPTB_TUSS_ID = 3814
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3816 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.390' WHERE PROPTB_TUSS_ID = 3815
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3817 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.477' WHERE PROPTB_TUSS_ID = 3816
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3818 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.560' WHERE PROPTB_TUSS_ID = 3817
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3819 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.653' WHERE PROPTB_TUSS_ID = 3818
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3820 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.750' WHERE PROPTB_TUSS_ID = 3819
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3821 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.837' WHERE PROPTB_TUSS_ID = 3820
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3822 , CLOUD_SYNC_DATE = '2014-05-06 17:38:12.917' WHERE PROPTB_TUSS_ID = 3821
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3823 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.000' WHERE PROPTB_TUSS_ID = 3822
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3824 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.083' WHERE PROPTB_TUSS_ID = 3823
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3825 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.170' WHERE PROPTB_TUSS_ID = 3824
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3826 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.257' WHERE PROPTB_TUSS_ID = 3825
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3827 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.340' WHERE PROPTB_TUSS_ID = 3826
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3828 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.427' WHERE PROPTB_TUSS_ID = 3827
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3829 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.513' WHERE PROPTB_TUSS_ID = 3828
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3830 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.597' WHERE PROPTB_TUSS_ID = 3829
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3831 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.687' WHERE PROPTB_TUSS_ID = 3830
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3832 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.770' WHERE PROPTB_TUSS_ID = 3831
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3833 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.857' WHERE PROPTB_TUSS_ID = 3832
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3834 , CLOUD_SYNC_DATE = '2014-05-06 17:38:13.940' WHERE PROPTB_TUSS_ID = 3833
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3835 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.027' WHERE PROPTB_TUSS_ID = 3834
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3836 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.110' WHERE PROPTB_TUSS_ID = 3835
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3837 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.197' WHERE PROPTB_TUSS_ID = 3836
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3838 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.280' WHERE PROPTB_TUSS_ID = 3837
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3839 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.363' WHERE PROPTB_TUSS_ID = 3838
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3840 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.450' WHERE PROPTB_TUSS_ID = 3839
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3841 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.543' WHERE PROPTB_TUSS_ID = 3840
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3842 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.627' WHERE PROPTB_TUSS_ID = 3841
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3843 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.717' WHERE PROPTB_TUSS_ID = 3842
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3844 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.800' WHERE PROPTB_TUSS_ID = 3843
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3845 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.887' WHERE PROPTB_TUSS_ID = 3844
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3846 , CLOUD_SYNC_DATE = '2014-05-06 17:38:14.967' WHERE PROPTB_TUSS_ID = 3845
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3847 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.053' WHERE PROPTB_TUSS_ID = 3846
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3848 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.143' WHERE PROPTB_TUSS_ID = 3847
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3849 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.230' WHERE PROPTB_TUSS_ID = 3848
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3850 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.313' WHERE PROPTB_TUSS_ID = 3849
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3851 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.400' WHERE PROPTB_TUSS_ID = 3850
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3852 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.500' WHERE PROPTB_TUSS_ID = 3851
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3853 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.587' WHERE PROPTB_TUSS_ID = 3852
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3854 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.680' WHERE PROPTB_TUSS_ID = 3853
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3855 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.763' WHERE PROPTB_TUSS_ID = 3854
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3856 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.847' WHERE PROPTB_TUSS_ID = 3855
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3857 , CLOUD_SYNC_DATE = '2014-05-06 17:38:15.933' WHERE PROPTB_TUSS_ID = 3856
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3858 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.017' WHERE PROPTB_TUSS_ID = 3857
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3859 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.103' WHERE PROPTB_TUSS_ID = 3858
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3860 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.217' WHERE PROPTB_TUSS_ID = 3859
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3861 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.303' WHERE PROPTB_TUSS_ID = 3860
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3862 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.390' WHERE PROPTB_TUSS_ID = 3861
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3863 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.477' WHERE PROPTB_TUSS_ID = 3862
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3864 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.560' WHERE PROPTB_TUSS_ID = 3863
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3865 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.643' WHERE PROPTB_TUSS_ID = 3864
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3866 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.733' WHERE PROPTB_TUSS_ID = 3865
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3867 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.820' WHERE PROPTB_TUSS_ID = 3866
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3868 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.907' WHERE PROPTB_TUSS_ID = 3867
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3869 , CLOUD_SYNC_DATE = '2014-05-06 17:38:16.990' WHERE PROPTB_TUSS_ID = 3868
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3870 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.077' WHERE PROPTB_TUSS_ID = 3869
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3871 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.163' WHERE PROPTB_TUSS_ID = 3870
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3872 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.257' WHERE PROPTB_TUSS_ID = 3871
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3873 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.340' WHERE PROPTB_TUSS_ID = 3872
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3874 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.427' WHERE PROPTB_TUSS_ID = 3873
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3875 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.513' WHERE PROPTB_TUSS_ID = 3874
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3876 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.597' WHERE PROPTB_TUSS_ID = 3875
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3877 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.687' WHERE PROPTB_TUSS_ID = 3876
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3878 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.777' WHERE PROPTB_TUSS_ID = 3877
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3879 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.870' WHERE PROPTB_TUSS_ID = 3878
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3880 , CLOUD_SYNC_DATE = '2014-05-06 17:38:17.960' WHERE PROPTB_TUSS_ID = 3879
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3881 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.050' WHERE PROPTB_TUSS_ID = 3880
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3882 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.137' WHERE PROPTB_TUSS_ID = 3881
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3883 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.233' WHERE PROPTB_TUSS_ID = 3882
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3884 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.323' WHERE PROPTB_TUSS_ID = 3883
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3885 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.410' WHERE PROPTB_TUSS_ID = 3884
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3886 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.503' WHERE PROPTB_TUSS_ID = 3885
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3887 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.590' WHERE PROPTB_TUSS_ID = 3886
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3888 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.683' WHERE PROPTB_TUSS_ID = 3887
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3889 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.767' WHERE PROPTB_TUSS_ID = 3888
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3890 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.850' WHERE PROPTB_TUSS_ID = 3889
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3891 , CLOUD_SYNC_DATE = '2014-05-06 17:38:18.943' WHERE PROPTB_TUSS_ID = 3890
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3892 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.037' WHERE PROPTB_TUSS_ID = 3891
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3893 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.127' WHERE PROPTB_TUSS_ID = 3892
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3894 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.213' WHERE PROPTB_TUSS_ID = 3893
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3895 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.300' WHERE PROPTB_TUSS_ID = 3894
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3896 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.383' WHERE PROPTB_TUSS_ID = 3895
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3897 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.470' WHERE PROPTB_TUSS_ID = 3896
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3898 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.560' WHERE PROPTB_TUSS_ID = 3897
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3899 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.647' WHERE PROPTB_TUSS_ID = 3898
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3900 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.733' WHERE PROPTB_TUSS_ID = 3899
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3901 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.817' WHERE PROPTB_TUSS_ID = 3900
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3902 , CLOUD_SYNC_DATE = '2014-05-06 17:38:19.913' WHERE PROPTB_TUSS_ID = 3901
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3903 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.003' WHERE PROPTB_TUSS_ID = 3902
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3904 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.090' WHERE PROPTB_TUSS_ID = 3903
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3905 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.177' WHERE PROPTB_TUSS_ID = 3904
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3906 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.263' WHERE PROPTB_TUSS_ID = 3905
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3907 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.353' WHERE PROPTB_TUSS_ID = 3906
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3908 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.443' WHERE PROPTB_TUSS_ID = 3907
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3909 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.530' WHERE PROPTB_TUSS_ID = 3908
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3910 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.617' WHERE PROPTB_TUSS_ID = 3909
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3911 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.707' WHERE PROPTB_TUSS_ID = 3910
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3912 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.797' WHERE PROPTB_TUSS_ID = 3911
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3913 , CLOUD_SYNC_DATE = '2014-05-06 17:38:20.910' WHERE PROPTB_TUSS_ID = 3912
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3914 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.003' WHERE PROPTB_TUSS_ID = 3913
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3915 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.090' WHERE PROPTB_TUSS_ID = 3914
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3916 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.177' WHERE PROPTB_TUSS_ID = 3915
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3917 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.270' WHERE PROPTB_TUSS_ID = 3916
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3918 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.357' WHERE PROPTB_TUSS_ID = 3917
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3919 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.447' WHERE PROPTB_TUSS_ID = 3918
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3920 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.530' WHERE PROPTB_TUSS_ID = 3919
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3921 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.620' WHERE PROPTB_TUSS_ID = 3920
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3922 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.707' WHERE PROPTB_TUSS_ID = 3921
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3923 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.793' WHERE PROPTB_TUSS_ID = 3922
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3924 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.880' WHERE PROPTB_TUSS_ID = 3923
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3925 , CLOUD_SYNC_DATE = '2014-05-06 17:38:21.980' WHERE PROPTB_TUSS_ID = 3924
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3926 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.067' WHERE PROPTB_TUSS_ID = 3925
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3927 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.157' WHERE PROPTB_TUSS_ID = 3926
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3928 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.250' WHERE PROPTB_TUSS_ID = 3927
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3929 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.340' WHERE PROPTB_TUSS_ID = 3928
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3930 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.430' WHERE PROPTB_TUSS_ID = 3929
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3931 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.517' WHERE PROPTB_TUSS_ID = 3930
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3932 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.613' WHERE PROPTB_TUSS_ID = 3931
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3933 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.750' WHERE PROPTB_TUSS_ID = 3932
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3934 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.853' WHERE PROPTB_TUSS_ID = 3933
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3935 , CLOUD_SYNC_DATE = '2014-05-06 17:38:22.943' WHERE PROPTB_TUSS_ID = 3934
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3936 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.037' WHERE PROPTB_TUSS_ID = 3935
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3937 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.130' WHERE PROPTB_TUSS_ID = 3936
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3938 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.227' WHERE PROPTB_TUSS_ID = 3937
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3939 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.327' WHERE PROPTB_TUSS_ID = 3938
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3940 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.423' WHERE PROPTB_TUSS_ID = 3939
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3941 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.520' WHERE PROPTB_TUSS_ID = 3940
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3942 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.613' WHERE PROPTB_TUSS_ID = 3941
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3943 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.717' WHERE PROPTB_TUSS_ID = 3942
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3944 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.803' WHERE PROPTB_TUSS_ID = 3943
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3945 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.893' WHERE PROPTB_TUSS_ID = 3944
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3946 , CLOUD_SYNC_DATE = '2014-05-06 17:38:23.987' WHERE PROPTB_TUSS_ID = 3945
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3947 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.087' WHERE PROPTB_TUSS_ID = 3946
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3948 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.183' WHERE PROPTB_TUSS_ID = 3947
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3949 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.303' WHERE PROPTB_TUSS_ID = 3948
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3950 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.403' WHERE PROPTB_TUSS_ID = 3949
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3951 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.517' WHERE PROPTB_TUSS_ID = 3950
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3952 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.610' WHERE PROPTB_TUSS_ID = 3951
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3953 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.713' WHERE PROPTB_TUSS_ID = 3952
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3954 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.820' WHERE PROPTB_TUSS_ID = 3953
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3955 , CLOUD_SYNC_DATE = '2014-05-06 17:38:24.953' WHERE PROPTB_TUSS_ID = 3954
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3956 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.073' WHERE PROPTB_TUSS_ID = 3955
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3957 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.187' WHERE PROPTB_TUSS_ID = 3956
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3958 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.307' WHERE PROPTB_TUSS_ID = 3957
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3959 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.407' WHERE PROPTB_TUSS_ID = 3958
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3960 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.517' WHERE PROPTB_TUSS_ID = 3959
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3961 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.747' WHERE PROPTB_TUSS_ID = 3960
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3962 , CLOUD_SYNC_DATE = '2014-05-06 17:38:25.913' WHERE PROPTB_TUSS_ID = 3961
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3963 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.047' WHERE PROPTB_TUSS_ID = 3962
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3964 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.137' WHERE PROPTB_TUSS_ID = 3963
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3965 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.227' WHERE PROPTB_TUSS_ID = 3964
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3966 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.313' WHERE PROPTB_TUSS_ID = 3965
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3967 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.407' WHERE PROPTB_TUSS_ID = 3966
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3968 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.497' WHERE PROPTB_TUSS_ID = 3967
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3969 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.583' WHERE PROPTB_TUSS_ID = 3968
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3970 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.670' WHERE PROPTB_TUSS_ID = 3969
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3971 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.757' WHERE PROPTB_TUSS_ID = 3970
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3972 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.847' WHERE PROPTB_TUSS_ID = 3971
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3973 , CLOUD_SYNC_DATE = '2014-05-06 17:38:26.940' WHERE PROPTB_TUSS_ID = 3972
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3974 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.027' WHERE PROPTB_TUSS_ID = 3973
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3975 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.117' WHERE PROPTB_TUSS_ID = 3974
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3976 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.207' WHERE PROPTB_TUSS_ID = 3975
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3977 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.293' WHERE PROPTB_TUSS_ID = 3976
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3978 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.380' WHERE PROPTB_TUSS_ID = 3977
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3979 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.467' WHERE PROPTB_TUSS_ID = 3978
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3980 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.557' WHERE PROPTB_TUSS_ID = 3979
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3981 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.643' WHERE PROPTB_TUSS_ID = 3980
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3982 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.743' WHERE PROPTB_TUSS_ID = 3981
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3983 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.830' WHERE PROPTB_TUSS_ID = 3982
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3984 , CLOUD_SYNC_DATE = '2014-05-06 17:38:27.917' WHERE PROPTB_TUSS_ID = 3983
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3985 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.007' WHERE PROPTB_TUSS_ID = 3984
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3986 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.097' WHERE PROPTB_TUSS_ID = 3985
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3987 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.187' WHERE PROPTB_TUSS_ID = 3986
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3988 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.273' WHERE PROPTB_TUSS_ID = 3987
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3989 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.367' WHERE PROPTB_TUSS_ID = 3988
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3990 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.453' WHERE PROPTB_TUSS_ID = 3989
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3991 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.537' WHERE PROPTB_TUSS_ID = 3990
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3992 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.627' WHERE PROPTB_TUSS_ID = 3991
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3993 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.720' WHERE PROPTB_TUSS_ID = 3992
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3994 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.807' WHERE PROPTB_TUSS_ID = 3993
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3995 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.910' WHERE PROPTB_TUSS_ID = 3994
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3996 , CLOUD_SYNC_DATE = '2014-05-06 17:38:28.997' WHERE PROPTB_TUSS_ID = 3995
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3997 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.083' WHERE PROPTB_TUSS_ID = 3996
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3998 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.170' WHERE PROPTB_TUSS_ID = 3997
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3999 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.267' WHERE PROPTB_TUSS_ID = 3998
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4000 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.353' WHERE PROPTB_TUSS_ID = 3999
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4001 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.443' WHERE PROPTB_TUSS_ID = 4000
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4002 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.537' WHERE PROPTB_TUSS_ID = 4001
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4003 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.627' WHERE PROPTB_TUSS_ID = 4002
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4004 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.717' WHERE PROPTB_TUSS_ID = 4003
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4005 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.810' WHERE PROPTB_TUSS_ID = 4004
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4006 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.900' WHERE PROPTB_TUSS_ID = 4005
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4007 , CLOUD_SYNC_DATE = '2014-05-06 17:38:29.990' WHERE PROPTB_TUSS_ID = 4006
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4008 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.080' WHERE PROPTB_TUSS_ID = 4007
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4009 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.170' WHERE PROPTB_TUSS_ID = 4008
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4010 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.260' WHERE PROPTB_TUSS_ID = 4009
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4011 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.353' WHERE PROPTB_TUSS_ID = 4010
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4012 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.443' WHERE PROPTB_TUSS_ID = 4011
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4013 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.533' WHERE PROPTB_TUSS_ID = 4012
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4014 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.627' WHERE PROPTB_TUSS_ID = 4013
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4015 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.720' WHERE PROPTB_TUSS_ID = 4014
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4016 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.807' WHERE PROPTB_TUSS_ID = 4015
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4017 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.900' WHERE PROPTB_TUSS_ID = 4016
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4018 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.990' WHERE PROPTB_TUSS_ID = 4017
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4019 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.080' WHERE PROPTB_TUSS_ID = 4018
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4020 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.173' WHERE PROPTB_TUSS_ID = 4019
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4021 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.263' WHERE PROPTB_TUSS_ID = 4020
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4022 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.350' WHERE PROPTB_TUSS_ID = 4021
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4023 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.447' WHERE PROPTB_TUSS_ID = 4022
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4024 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.533' WHERE PROPTB_TUSS_ID = 4023
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4025 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.623' WHERE PROPTB_TUSS_ID = 4024
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4026 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.723' WHERE PROPTB_TUSS_ID = 4025
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4027 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.813' WHERE PROPTB_TUSS_ID = 4026
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4028 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.903' WHERE PROPTB_TUSS_ID = 4027
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4029 , CLOUD_SYNC_DATE = '2014-05-06 17:38:31.993' WHERE PROPTB_TUSS_ID = 4028
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4030 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.083' WHERE PROPTB_TUSS_ID = 4029
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4031 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.170' WHERE PROPTB_TUSS_ID = 4030
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4032 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.267' WHERE PROPTB_TUSS_ID = 4031
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4033 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.353' WHERE PROPTB_TUSS_ID = 4032
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4034 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.443' WHERE PROPTB_TUSS_ID = 4033
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4035 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.537' WHERE PROPTB_TUSS_ID = 4034
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4036 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.627' WHERE PROPTB_TUSS_ID = 4035
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4037 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.720' WHERE PROPTB_TUSS_ID = 4036
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4038 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.817' WHERE PROPTB_TUSS_ID = 4037
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4039 , CLOUD_SYNC_DATE = '2014-05-06 17:38:32.920' WHERE PROPTB_TUSS_ID = 4038
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4040 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.017' WHERE PROPTB_TUSS_ID = 4039
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4041 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.107' WHERE PROPTB_TUSS_ID = 4040
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4042 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.197' WHERE PROPTB_TUSS_ID = 4041
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4043 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.297' WHERE PROPTB_TUSS_ID = 4042
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4044 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.387' WHERE PROPTB_TUSS_ID = 4043
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4045 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.477' WHERE PROPTB_TUSS_ID = 4044
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4046 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.570' WHERE PROPTB_TUSS_ID = 4045
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4047 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.667' WHERE PROPTB_TUSS_ID = 4046
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4048 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.753' WHERE PROPTB_TUSS_ID = 4047
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4049 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.847' WHERE PROPTB_TUSS_ID = 4048
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4050 , CLOUD_SYNC_DATE = '2014-05-06 17:38:33.940' WHERE PROPTB_TUSS_ID = 4049
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4051 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.033' WHERE PROPTB_TUSS_ID = 4050
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4052 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.123' WHERE PROPTB_TUSS_ID = 4051
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4053 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.210' WHERE PROPTB_TUSS_ID = 4052
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4054 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.300' WHERE PROPTB_TUSS_ID = 4053
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4055 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.390' WHERE PROPTB_TUSS_ID = 4054
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4056 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.480' WHERE PROPTB_TUSS_ID = 4055
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4057 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.570' WHERE PROPTB_TUSS_ID = 4056
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4058 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.663' WHERE PROPTB_TUSS_ID = 4057
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4059 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.753' WHERE PROPTB_TUSS_ID = 4058
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4060 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.843' WHERE PROPTB_TUSS_ID = 4059
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4061 , CLOUD_SYNC_DATE = '2014-05-06 17:38:34.937' WHERE PROPTB_TUSS_ID = 4060
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4062 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.023' WHERE PROPTB_TUSS_ID = 4061
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4063 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.113' WHERE PROPTB_TUSS_ID = 4062
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4064 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.203' WHERE PROPTB_TUSS_ID = 4063
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4065 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.297' WHERE PROPTB_TUSS_ID = 4064
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4066 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.390' WHERE PROPTB_TUSS_ID = 4065
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4067 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.477' WHERE PROPTB_TUSS_ID = 4066
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4068 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.567' WHERE PROPTB_TUSS_ID = 4067
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4069 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.690' WHERE PROPTB_TUSS_ID = 4068
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4070 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.803' WHERE PROPTB_TUSS_ID = 4069
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4071 , CLOUD_SYNC_DATE = '2014-05-06 17:38:35.903' WHERE PROPTB_TUSS_ID = 4070
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4072 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.020' WHERE PROPTB_TUSS_ID = 4071
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4073 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.117' WHERE PROPTB_TUSS_ID = 4072
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4074 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.220' WHERE PROPTB_TUSS_ID = 4073
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4075 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.357' WHERE PROPTB_TUSS_ID = 4074
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4076 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.453' WHERE PROPTB_TUSS_ID = 4075
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4077 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.547' WHERE PROPTB_TUSS_ID = 4076
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4078 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.640' WHERE PROPTB_TUSS_ID = 4077
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4079 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.737' WHERE PROPTB_TUSS_ID = 4078
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4080 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.827' WHERE PROPTB_TUSS_ID = 4079
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4081 , CLOUD_SYNC_DATE = '2014-05-06 17:38:36.917' WHERE PROPTB_TUSS_ID = 4080
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4082 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.010' WHERE PROPTB_TUSS_ID = 4081
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4083 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.100' WHERE PROPTB_TUSS_ID = 4082
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4084 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.193' WHERE PROPTB_TUSS_ID = 4083
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4085 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.287' WHERE PROPTB_TUSS_ID = 4084
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4086 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.373' WHERE PROPTB_TUSS_ID = 4085
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4087 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.467' WHERE PROPTB_TUSS_ID = 4086
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4088 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.557' WHERE PROPTB_TUSS_ID = 4087
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4089 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.650' WHERE PROPTB_TUSS_ID = 4088
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4090 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.743' WHERE PROPTB_TUSS_ID = 4089
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4091 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.837' WHERE PROPTB_TUSS_ID = 4090
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4092 , CLOUD_SYNC_DATE = '2014-05-06 17:38:37.927' WHERE PROPTB_TUSS_ID = 4091
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4093 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.017' WHERE PROPTB_TUSS_ID = 4092
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4094 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.110' WHERE PROPTB_TUSS_ID = 4093
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4095 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.200' WHERE PROPTB_TUSS_ID = 4094
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4096 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.293' WHERE PROPTB_TUSS_ID = 4095
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4097 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.383' WHERE PROPTB_TUSS_ID = 4096
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4098 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.473' WHERE PROPTB_TUSS_ID = 4097
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4099 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.567' WHERE PROPTB_TUSS_ID = 4098
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4100 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.657' WHERE PROPTB_TUSS_ID = 4099
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4101 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.750' WHERE PROPTB_TUSS_ID = 4100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4102 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.840' WHERE PROPTB_TUSS_ID = 4101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4103 , CLOUD_SYNC_DATE = '2014-05-06 17:38:38.933' WHERE PROPTB_TUSS_ID = 4102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4104 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.023' WHERE PROPTB_TUSS_ID = 4103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4105 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.110' WHERE PROPTB_TUSS_ID = 4104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4106 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.203' WHERE PROPTB_TUSS_ID = 4105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4107 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.297' WHERE PROPTB_TUSS_ID = 4106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4108 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.390' WHERE PROPTB_TUSS_ID = 4107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4109 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.480' WHERE PROPTB_TUSS_ID = 4108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4110 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.577' WHERE PROPTB_TUSS_ID = 4109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4111 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.667' WHERE PROPTB_TUSS_ID = 4110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4112 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.757' WHERE PROPTB_TUSS_ID = 4111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4113 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.853' WHERE PROPTB_TUSS_ID = 4112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4114 , CLOUD_SYNC_DATE = '2014-05-06 17:38:39.943' WHERE PROPTB_TUSS_ID = 4113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4115 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.037' WHERE PROPTB_TUSS_ID = 4114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4116 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.130' WHERE PROPTB_TUSS_ID = 4115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4117 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.223' WHERE PROPTB_TUSS_ID = 4116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4118 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.313' WHERE PROPTB_TUSS_ID = 4117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4119 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.403' WHERE PROPTB_TUSS_ID = 4118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4120 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.497' WHERE PROPTB_TUSS_ID = 4119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4121 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.587' WHERE PROPTB_TUSS_ID = 4120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4122 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.687' WHERE PROPTB_TUSS_ID = 4121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4123 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.777' WHERE PROPTB_TUSS_ID = 4122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4124 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.887' WHERE PROPTB_TUSS_ID = 4123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4125 , CLOUD_SYNC_DATE = '2014-05-06 17:38:40.977' WHERE PROPTB_TUSS_ID = 4124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4126 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.067' WHERE PROPTB_TUSS_ID = 4125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4127 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.157' WHERE PROPTB_TUSS_ID = 4126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4128 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.247' WHERE PROPTB_TUSS_ID = 4127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4129 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.343' WHERE PROPTB_TUSS_ID = 4128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4130 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.437' WHERE PROPTB_TUSS_ID = 4129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4131 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.530' WHERE PROPTB_TUSS_ID = 4130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4132 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.620' WHERE PROPTB_TUSS_ID = 4131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4133 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.720' WHERE PROPTB_TUSS_ID = 4132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4134 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.813' WHERE PROPTB_TUSS_ID = 4133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4135 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.907' WHERE PROPTB_TUSS_ID = 4134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4136 , CLOUD_SYNC_DATE = '2014-05-06 17:38:41.997' WHERE PROPTB_TUSS_ID = 4135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4137 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.090' WHERE PROPTB_TUSS_ID = 4136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4138 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.180' WHERE PROPTB_TUSS_ID = 4137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4139 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.277' WHERE PROPTB_TUSS_ID = 4138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4140 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.367' WHERE PROPTB_TUSS_ID = 4139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4141 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.457' WHERE PROPTB_TUSS_ID = 4140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4142 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.550' WHERE PROPTB_TUSS_ID = 4141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4143 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.643' WHERE PROPTB_TUSS_ID = 4142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4144 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.740' WHERE PROPTB_TUSS_ID = 4143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4145 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.833' WHERE PROPTB_TUSS_ID = 4144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4146 , CLOUD_SYNC_DATE = '2014-05-06 17:38:42.937' WHERE PROPTB_TUSS_ID = 4145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4147 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.030' WHERE PROPTB_TUSS_ID = 4146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4148 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.123' WHERE PROPTB_TUSS_ID = 4147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4149 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.213' WHERE PROPTB_TUSS_ID = 4148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4150 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.307' WHERE PROPTB_TUSS_ID = 4149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4151 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.400' WHERE PROPTB_TUSS_ID = 4150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4152 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.493' WHERE PROPTB_TUSS_ID = 4151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4153 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.583' WHERE PROPTB_TUSS_ID = 4152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4154 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.683' WHERE PROPTB_TUSS_ID = 4153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4155 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.773' WHERE PROPTB_TUSS_ID = 4154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4156 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.870' WHERE PROPTB_TUSS_ID = 4155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4157 , CLOUD_SYNC_DATE = '2014-05-06 17:38:43.960' WHERE PROPTB_TUSS_ID = 4156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4158 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.053' WHERE PROPTB_TUSS_ID = 4157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4159 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.147' WHERE PROPTB_TUSS_ID = 4158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4160 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.243' WHERE PROPTB_TUSS_ID = 4159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4161 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.337' WHERE PROPTB_TUSS_ID = 4160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4162 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.433' WHERE PROPTB_TUSS_ID = 4161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4163 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.530' WHERE PROPTB_TUSS_ID = 4162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4164 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.623' WHERE PROPTB_TUSS_ID = 4163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4165 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.720' WHERE PROPTB_TUSS_ID = 4164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4166 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.813' WHERE PROPTB_TUSS_ID = 4165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4167 , CLOUD_SYNC_DATE = '2014-05-06 17:38:44.910' WHERE PROPTB_TUSS_ID = 4166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4168 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.007' WHERE PROPTB_TUSS_ID = 4167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4169 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.107' WHERE PROPTB_TUSS_ID = 4168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4170 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.197' WHERE PROPTB_TUSS_ID = 4169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4171 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.297' WHERE PROPTB_TUSS_ID = 4170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4172 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.390' WHERE PROPTB_TUSS_ID = 4171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4173 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.487' WHERE PROPTB_TUSS_ID = 4172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4174 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.580' WHERE PROPTB_TUSS_ID = 4173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4175 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.677' WHERE PROPTB_TUSS_ID = 4174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4176 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.767' WHERE PROPTB_TUSS_ID = 4175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4177 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.860' WHERE PROPTB_TUSS_ID = 4176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4178 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.950' WHERE PROPTB_TUSS_ID = 4177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4179 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.053' WHERE PROPTB_TUSS_ID = 4178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4180 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.150' WHERE PROPTB_TUSS_ID = 4179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4181 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.247' WHERE PROPTB_TUSS_ID = 4180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4182 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.343' WHERE PROPTB_TUSS_ID = 4181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4183 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.440' WHERE PROPTB_TUSS_ID = 4182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4184 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.537' WHERE PROPTB_TUSS_ID = 4183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4185 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.627' WHERE PROPTB_TUSS_ID = 4184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4186 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.727' WHERE PROPTB_TUSS_ID = 4185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4187 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.817' WHERE PROPTB_TUSS_ID = 4186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4188 , CLOUD_SYNC_DATE = '2014-05-06 17:38:46.917' WHERE PROPTB_TUSS_ID = 4187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4189 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.007' WHERE PROPTB_TUSS_ID = 4188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4190 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.133' WHERE PROPTB_TUSS_ID = 4189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4191 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.267' WHERE PROPTB_TUSS_ID = 4190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4192 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.370' WHERE PROPTB_TUSS_ID = 4191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4193 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.477' WHERE PROPTB_TUSS_ID = 4192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4194 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.587' WHERE PROPTB_TUSS_ID = 4193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4195 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.687' WHERE PROPTB_TUSS_ID = 4194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4196 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.787' WHERE PROPTB_TUSS_ID = 4195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4197 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.887' WHERE PROPTB_TUSS_ID = 4196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4198 , CLOUD_SYNC_DATE = '2014-05-06 17:38:47.987' WHERE PROPTB_TUSS_ID = 4197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4199 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.083' WHERE PROPTB_TUSS_ID = 4198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4200 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.180' WHERE PROPTB_TUSS_ID = 4199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4201 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.277' WHERE PROPTB_TUSS_ID = 4200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4202 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.377' WHERE PROPTB_TUSS_ID = 4201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4203 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.470' WHERE PROPTB_TUSS_ID = 4202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4204 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.580' WHERE PROPTB_TUSS_ID = 4203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4205 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.683' WHERE PROPTB_TUSS_ID = 4204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4206 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.783' WHERE PROPTB_TUSS_ID = 4205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4207 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.880' WHERE PROPTB_TUSS_ID = 4206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4208 , CLOUD_SYNC_DATE = '2014-05-06 17:38:48.977' WHERE PROPTB_TUSS_ID = 4207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4209 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.073' WHERE PROPTB_TUSS_ID = 4208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4210 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.167' WHERE PROPTB_TUSS_ID = 4209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4211 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.267' WHERE PROPTB_TUSS_ID = 4210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4212 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.363' WHERE PROPTB_TUSS_ID = 4211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4213 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.457' WHERE PROPTB_TUSS_ID = 4212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4214 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.550' WHERE PROPTB_TUSS_ID = 4213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4215 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.663' WHERE PROPTB_TUSS_ID = 4214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4216 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.763' WHERE PROPTB_TUSS_ID = 4215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4217 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.860' WHERE PROPTB_TUSS_ID = 4216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4218 , CLOUD_SYNC_DATE = '2014-05-06 17:38:49.960' WHERE PROPTB_TUSS_ID = 4217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4219 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.057' WHERE PROPTB_TUSS_ID = 4218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4220 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.153' WHERE PROPTB_TUSS_ID = 4219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4221 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.247' WHERE PROPTB_TUSS_ID = 4220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4222 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.350' WHERE PROPTB_TUSS_ID = 4221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4223 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.447' WHERE PROPTB_TUSS_ID = 4222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4224 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.543' WHERE PROPTB_TUSS_ID = 4223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4225 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.643' WHERE PROPTB_TUSS_ID = 4224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4226 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.747' WHERE PROPTB_TUSS_ID = 4225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4227 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.843' WHERE PROPTB_TUSS_ID = 4226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4228 , CLOUD_SYNC_DATE = '2014-05-06 17:38:50.943' WHERE PROPTB_TUSS_ID = 4227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4229 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.043' WHERE PROPTB_TUSS_ID = 4228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4230 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.137' WHERE PROPTB_TUSS_ID = 4229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4231 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.237' WHERE PROPTB_TUSS_ID = 4230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4232 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.333' WHERE PROPTB_TUSS_ID = 4231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4233 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.437' WHERE PROPTB_TUSS_ID = 4232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4234 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.530' WHERE PROPTB_TUSS_ID = 4233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4235 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.633' WHERE PROPTB_TUSS_ID = 4234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4236 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.733' WHERE PROPTB_TUSS_ID = 4235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4237 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.827' WHERE PROPTB_TUSS_ID = 4236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4238 , CLOUD_SYNC_DATE = '2014-05-06 17:38:51.927' WHERE PROPTB_TUSS_ID = 4237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4239 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.020' WHERE PROPTB_TUSS_ID = 4238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4240 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.117' WHERE PROPTB_TUSS_ID = 4239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4241 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.213' WHERE PROPTB_TUSS_ID = 4240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4242 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.313' WHERE PROPTB_TUSS_ID = 4241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4243 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.413' WHERE PROPTB_TUSS_ID = 4242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4244 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.507' WHERE PROPTB_TUSS_ID = 4243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4245 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.603' WHERE PROPTB_TUSS_ID = 4244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4246 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.703' WHERE PROPTB_TUSS_ID = 4245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4247 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.800' WHERE PROPTB_TUSS_ID = 4246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4248 , CLOUD_SYNC_DATE = '2014-05-06 17:38:52.897' WHERE PROPTB_TUSS_ID = 4247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4249 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.003' WHERE PROPTB_TUSS_ID = 4248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4250 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.107' WHERE PROPTB_TUSS_ID = 4249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4251 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.203' WHERE PROPTB_TUSS_ID = 4250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4252 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.300' WHERE PROPTB_TUSS_ID = 4251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4253 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.397' WHERE PROPTB_TUSS_ID = 4252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4254 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.497' WHERE PROPTB_TUSS_ID = 4253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4255 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.597' WHERE PROPTB_TUSS_ID = 4254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4256 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.707' WHERE PROPTB_TUSS_ID = 4255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4257 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.807' WHERE PROPTB_TUSS_ID = 4256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4258 , CLOUD_SYNC_DATE = '2014-05-06 17:38:53.903' WHERE PROPTB_TUSS_ID = 4257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4259 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.003' WHERE PROPTB_TUSS_ID = 4258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4260 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.100' WHERE PROPTB_TUSS_ID = 4259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4261 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.197' WHERE PROPTB_TUSS_ID = 4260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4262 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.300' WHERE PROPTB_TUSS_ID = 4261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4263 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.397' WHERE PROPTB_TUSS_ID = 4262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4264 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.497' WHERE PROPTB_TUSS_ID = 4263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4265 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.597' WHERE PROPTB_TUSS_ID = 4264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4266 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.697' WHERE PROPTB_TUSS_ID = 4265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4267 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.797' WHERE PROPTB_TUSS_ID = 4266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4268 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.890' WHERE PROPTB_TUSS_ID = 4267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4269 , CLOUD_SYNC_DATE = '2014-05-06 17:38:54.997' WHERE PROPTB_TUSS_ID = 4268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4270 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.097' WHERE PROPTB_TUSS_ID = 4269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4271 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.193' WHERE PROPTB_TUSS_ID = 4270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4272 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.293' WHERE PROPTB_TUSS_ID = 4271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4273 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.390' WHERE PROPTB_TUSS_ID = 4272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4274 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.493' WHERE PROPTB_TUSS_ID = 4273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4275 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.593' WHERE PROPTB_TUSS_ID = 4274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4276 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.690' WHERE PROPTB_TUSS_ID = 4275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4277 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.790' WHERE PROPTB_TUSS_ID = 4276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4278 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.883' WHERE PROPTB_TUSS_ID = 4277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4279 , CLOUD_SYNC_DATE = '2014-05-06 17:38:55.983' WHERE PROPTB_TUSS_ID = 4278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4280 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.080' WHERE PROPTB_TUSS_ID = 4279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4281 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.173' WHERE PROPTB_TUSS_ID = 4280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4282 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.270' WHERE PROPTB_TUSS_ID = 4281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4283 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.367' WHERE PROPTB_TUSS_ID = 4282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4284 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.463' WHERE PROPTB_TUSS_ID = 4283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4285 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.567' WHERE PROPTB_TUSS_ID = 4284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4286 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.667' WHERE PROPTB_TUSS_ID = 4285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4287 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.763' WHERE PROPTB_TUSS_ID = 4286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4288 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.863' WHERE PROPTB_TUSS_ID = 4287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4289 , CLOUD_SYNC_DATE = '2014-05-06 17:38:56.957' WHERE PROPTB_TUSS_ID = 4288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4290 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.057' WHERE PROPTB_TUSS_ID = 4289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4291 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.147' WHERE PROPTB_TUSS_ID = 4290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4292 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.247' WHERE PROPTB_TUSS_ID = 4291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4293 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.343' WHERE PROPTB_TUSS_ID = 4292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4294 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.440' WHERE PROPTB_TUSS_ID = 4293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4295 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.533' WHERE PROPTB_TUSS_ID = 4294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4296 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.633' WHERE PROPTB_TUSS_ID = 4295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4297 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.730' WHERE PROPTB_TUSS_ID = 4296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4298 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.827' WHERE PROPTB_TUSS_ID = 4297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4299 , CLOUD_SYNC_DATE = '2014-05-06 17:38:57.923' WHERE PROPTB_TUSS_ID = 4298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4300 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.023' WHERE PROPTB_TUSS_ID = 4299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4301 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.123' WHERE PROPTB_TUSS_ID = 4300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4302 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.217' WHERE PROPTB_TUSS_ID = 4301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4303 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.313' WHERE PROPTB_TUSS_ID = 4302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4304 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.413' WHERE PROPTB_TUSS_ID = 4303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4305 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.507' WHERE PROPTB_TUSS_ID = 4304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4306 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.607' WHERE PROPTB_TUSS_ID = 4305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4307 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.703' WHERE PROPTB_TUSS_ID = 4306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4308 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.800' WHERE PROPTB_TUSS_ID = 4307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4309 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.897' WHERE PROPTB_TUSS_ID = 4308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4310 , CLOUD_SYNC_DATE = '2014-05-06 17:38:58.997' WHERE PROPTB_TUSS_ID = 4309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4311 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.103' WHERE PROPTB_TUSS_ID = 4310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4312 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.203' WHERE PROPTB_TUSS_ID = 4311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4313 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.300' WHERE PROPTB_TUSS_ID = 4312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4314 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.397' WHERE PROPTB_TUSS_ID = 4313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4315 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.493' WHERE PROPTB_TUSS_ID = 4314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4316 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.593' WHERE PROPTB_TUSS_ID = 4315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4317 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.690' WHERE PROPTB_TUSS_ID = 4316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4318 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.790' WHERE PROPTB_TUSS_ID = 4317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4319 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.887' WHERE PROPTB_TUSS_ID = 4318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4320 , CLOUD_SYNC_DATE = '2014-05-06 17:38:59.983' WHERE PROPTB_TUSS_ID = 4319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4321 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.080' WHERE PROPTB_TUSS_ID = 4320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4322 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.177' WHERE PROPTB_TUSS_ID = 4321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4323 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.273' WHERE PROPTB_TUSS_ID = 4322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4324 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.373' WHERE PROPTB_TUSS_ID = 4323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4325 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.470' WHERE PROPTB_TUSS_ID = 4324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4326 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.567' WHERE PROPTB_TUSS_ID = 4325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4327 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.667' WHERE PROPTB_TUSS_ID = 4326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4328 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.780' WHERE PROPTB_TUSS_ID = 4327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4329 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.877' WHERE PROPTB_TUSS_ID = 4328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4330 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.977' WHERE PROPTB_TUSS_ID = 4329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4331 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.077' WHERE PROPTB_TUSS_ID = 4330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4332 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.173' WHERE PROPTB_TUSS_ID = 4331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4333 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.267' WHERE PROPTB_TUSS_ID = 4332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4334 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.367' WHERE PROPTB_TUSS_ID = 4333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4335 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.460' WHERE PROPTB_TUSS_ID = 4334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4336 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.557' WHERE PROPTB_TUSS_ID = 4335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4337 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.657' WHERE PROPTB_TUSS_ID = 4336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4338 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.753' WHERE PROPTB_TUSS_ID = 4337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4339 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.853' WHERE PROPTB_TUSS_ID = 4338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4340 , CLOUD_SYNC_DATE = '2014-05-06 17:39:01.950' WHERE PROPTB_TUSS_ID = 4339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4341 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.043' WHERE PROPTB_TUSS_ID = 4340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4342 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.143' WHERE PROPTB_TUSS_ID = 4341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4343 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.243' WHERE PROPTB_TUSS_ID = 4342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4344 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.337' WHERE PROPTB_TUSS_ID = 4343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4345 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.437' WHERE PROPTB_TUSS_ID = 4344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4346 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.533' WHERE PROPTB_TUSS_ID = 4345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4347 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.633' WHERE PROPTB_TUSS_ID = 4346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4348 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.733' WHERE PROPTB_TUSS_ID = 4347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4349 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.830' WHERE PROPTB_TUSS_ID = 4348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4350 , CLOUD_SYNC_DATE = '2014-05-06 17:39:02.930' WHERE PROPTB_TUSS_ID = 4349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4351 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.027' WHERE PROPTB_TUSS_ID = 4350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4352 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.137' WHERE PROPTB_TUSS_ID = 4351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4353 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.237' WHERE PROPTB_TUSS_ID = 4352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4354 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.333' WHERE PROPTB_TUSS_ID = 4353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4355 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.430' WHERE PROPTB_TUSS_ID = 4354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4356 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.530' WHERE PROPTB_TUSS_ID = 4355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4357 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.627' WHERE PROPTB_TUSS_ID = 4356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4358 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.737' WHERE PROPTB_TUSS_ID = 4357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4359 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.837' WHERE PROPTB_TUSS_ID = 4358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4360 , CLOUD_SYNC_DATE = '2014-05-06 17:39:03.933' WHERE PROPTB_TUSS_ID = 4359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4361 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.030' WHERE PROPTB_TUSS_ID = 4360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4362 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.137' WHERE PROPTB_TUSS_ID = 4361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4363 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.237' WHERE PROPTB_TUSS_ID = 4362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4364 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.327' WHERE PROPTB_TUSS_ID = 4363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4365 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.427' WHERE PROPTB_TUSS_ID = 4364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4366 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.527' WHERE PROPTB_TUSS_ID = 4365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4367 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.620' WHERE PROPTB_TUSS_ID = 4366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4368 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.723' WHERE PROPTB_TUSS_ID = 4367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4369 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.823' WHERE PROPTB_TUSS_ID = 4368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4370 , CLOUD_SYNC_DATE = '2014-05-06 17:39:04.920' WHERE PROPTB_TUSS_ID = 4369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4371 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.017' WHERE PROPTB_TUSS_ID = 4370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4372 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.117' WHERE PROPTB_TUSS_ID = 4371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4373 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.217' WHERE PROPTB_TUSS_ID = 4372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4374 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.310' WHERE PROPTB_TUSS_ID = 4373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4375 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.413' WHERE PROPTB_TUSS_ID = 4374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4376 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.510' WHERE PROPTB_TUSS_ID = 4375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4377 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.610' WHERE PROPTB_TUSS_ID = 4376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4378 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.710' WHERE PROPTB_TUSS_ID = 4377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4379 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.807' WHERE PROPTB_TUSS_ID = 4378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4380 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.907' WHERE PROPTB_TUSS_ID = 4379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4381 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.007' WHERE PROPTB_TUSS_ID = 4380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4382 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.103' WHERE PROPTB_TUSS_ID = 4381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4383 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.203' WHERE PROPTB_TUSS_ID = 4382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4384 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.303' WHERE PROPTB_TUSS_ID = 4383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4385 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.403' WHERE PROPTB_TUSS_ID = 4384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4386 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.507' WHERE PROPTB_TUSS_ID = 4385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4387 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.607' WHERE PROPTB_TUSS_ID = 4386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4388 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.710' WHERE PROPTB_TUSS_ID = 4387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4389 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.803' WHERE PROPTB_TUSS_ID = 4388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4390 , CLOUD_SYNC_DATE = '2014-05-06 17:39:06.907' WHERE PROPTB_TUSS_ID = 4389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4391 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.003' WHERE PROPTB_TUSS_ID = 4390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4392 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.103' WHERE PROPTB_TUSS_ID = 4391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4393 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.200' WHERE PROPTB_TUSS_ID = 4392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4394 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.300' WHERE PROPTB_TUSS_ID = 4393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4395 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.400' WHERE PROPTB_TUSS_ID = 4394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4396 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.500' WHERE PROPTB_TUSS_ID = 4395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4397 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.597' WHERE PROPTB_TUSS_ID = 4396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4398 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.697' WHERE PROPTB_TUSS_ID = 4397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4399 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.797' WHERE PROPTB_TUSS_ID = 4398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4400 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.897' WHERE PROPTB_TUSS_ID = 4399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4401 , CLOUD_SYNC_DATE = '2014-05-06 17:39:07.993' WHERE PROPTB_TUSS_ID = 4400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4402 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.090' WHERE PROPTB_TUSS_ID = 4401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4403 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.190' WHERE PROPTB_TUSS_ID = 4402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4404 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.297' WHERE PROPTB_TUSS_ID = 4403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4405 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.397' WHERE PROPTB_TUSS_ID = 4404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4406 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.497' WHERE PROPTB_TUSS_ID = 4405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4407 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.600' WHERE PROPTB_TUSS_ID = 4406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4408 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.703' WHERE PROPTB_TUSS_ID = 4407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4409 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.803' WHERE PROPTB_TUSS_ID = 4408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4410 , CLOUD_SYNC_DATE = '2014-05-06 17:39:08.907' WHERE PROPTB_TUSS_ID = 4409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4411 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.007' WHERE PROPTB_TUSS_ID = 4410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4412 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.107' WHERE PROPTB_TUSS_ID = 4411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4413 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.213' WHERE PROPTB_TUSS_ID = 4412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4414 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.317' WHERE PROPTB_TUSS_ID = 4413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4415 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.420' WHERE PROPTB_TUSS_ID = 4414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4416 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.523' WHERE PROPTB_TUSS_ID = 4415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4417 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.623' WHERE PROPTB_TUSS_ID = 4416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4418 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.727' WHERE PROPTB_TUSS_ID = 4417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4419 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.827' WHERE PROPTB_TUSS_ID = 4418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4420 , CLOUD_SYNC_DATE = '2014-05-06 17:39:09.923' WHERE PROPTB_TUSS_ID = 4419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4421 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.023' WHERE PROPTB_TUSS_ID = 4420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4422 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.123' WHERE PROPTB_TUSS_ID = 4421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4423 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.227' WHERE PROPTB_TUSS_ID = 4422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4424 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.327' WHERE PROPTB_TUSS_ID = 4423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4425 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.423' WHERE PROPTB_TUSS_ID = 4424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4426 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.523' WHERE PROPTB_TUSS_ID = 4425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4427 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.627' WHERE PROPTB_TUSS_ID = 4426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4428 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.727' WHERE PROPTB_TUSS_ID = 4427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4429 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.823' WHERE PROPTB_TUSS_ID = 4428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4430 , CLOUD_SYNC_DATE = '2014-05-06 17:39:10.927' WHERE PROPTB_TUSS_ID = 4429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4431 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.027' WHERE PROPTB_TUSS_ID = 4430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4432 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.127' WHERE PROPTB_TUSS_ID = 4431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4433 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.227' WHERE PROPTB_TUSS_ID = 4432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4434 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.327' WHERE PROPTB_TUSS_ID = 4433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4435 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.430' WHERE PROPTB_TUSS_ID = 4434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4436 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.533' WHERE PROPTB_TUSS_ID = 4435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4437 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.633' WHERE PROPTB_TUSS_ID = 4436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4438 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.733' WHERE PROPTB_TUSS_ID = 4437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4439 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.833' WHERE PROPTB_TUSS_ID = 4438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4440 , CLOUD_SYNC_DATE = '2014-05-06 17:39:11.937' WHERE PROPTB_TUSS_ID = 4439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4441 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.037' WHERE PROPTB_TUSS_ID = 4440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4442 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.137' WHERE PROPTB_TUSS_ID = 4441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4443 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.237' WHERE PROPTB_TUSS_ID = 4442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4444 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.337' WHERE PROPTB_TUSS_ID = 4443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4445 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.437' WHERE PROPTB_TUSS_ID = 4444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4446 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.537' WHERE PROPTB_TUSS_ID = 4445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4447 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.640' WHERE PROPTB_TUSS_ID = 4446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4448 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.740' WHERE PROPTB_TUSS_ID = 4447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4449 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.843' WHERE PROPTB_TUSS_ID = 4448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4450 , CLOUD_SYNC_DATE = '2014-05-06 17:39:12.943' WHERE PROPTB_TUSS_ID = 4449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4451 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.043' WHERE PROPTB_TUSS_ID = 4450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4452 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.210' WHERE PROPTB_TUSS_ID = 4451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4453 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.313' WHERE PROPTB_TUSS_ID = 4452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4454 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.417' WHERE PROPTB_TUSS_ID = 4453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4455 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.520' WHERE PROPTB_TUSS_ID = 4454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4456 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.623' WHERE PROPTB_TUSS_ID = 4455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4457 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.730' WHERE PROPTB_TUSS_ID = 4456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4458 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.827' WHERE PROPTB_TUSS_ID = 4457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4459 , CLOUD_SYNC_DATE = '2014-05-06 17:39:13.930' WHERE PROPTB_TUSS_ID = 4458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4460 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.030' WHERE PROPTB_TUSS_ID = 4459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4461 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.130' WHERE PROPTB_TUSS_ID = 4460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4462 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.230' WHERE PROPTB_TUSS_ID = 4461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4463 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.327' WHERE PROPTB_TUSS_ID = 4462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4464 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.430' WHERE PROPTB_TUSS_ID = 4463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4465 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.530' WHERE PROPTB_TUSS_ID = 4464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4466 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.633' WHERE PROPTB_TUSS_ID = 4465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4467 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.740' WHERE PROPTB_TUSS_ID = 4466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4468 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.840' WHERE PROPTB_TUSS_ID = 4467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4469 , CLOUD_SYNC_DATE = '2014-05-06 17:39:14.937' WHERE PROPTB_TUSS_ID = 4468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4470 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.040' WHERE PROPTB_TUSS_ID = 4469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4471 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.140' WHERE PROPTB_TUSS_ID = 4470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4472 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.243' WHERE PROPTB_TUSS_ID = 4471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4473 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.343' WHERE PROPTB_TUSS_ID = 4472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4474 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.447' WHERE PROPTB_TUSS_ID = 4473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4475 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.550' WHERE PROPTB_TUSS_ID = 4474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4476 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.653' WHERE PROPTB_TUSS_ID = 4475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4477 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.757' WHERE PROPTB_TUSS_ID = 4476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4478 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.863' WHERE PROPTB_TUSS_ID = 4477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4479 , CLOUD_SYNC_DATE = '2014-05-06 17:39:15.967' WHERE PROPTB_TUSS_ID = 4478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4480 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.067' WHERE PROPTB_TUSS_ID = 4479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4481 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.167' WHERE PROPTB_TUSS_ID = 4480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4482 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.270' WHERE PROPTB_TUSS_ID = 4481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4483 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.373' WHERE PROPTB_TUSS_ID = 4482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4484 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.477' WHERE PROPTB_TUSS_ID = 4483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4485 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.580' WHERE PROPTB_TUSS_ID = 4484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4486 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.683' WHERE PROPTB_TUSS_ID = 4485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4487 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.783' WHERE PROPTB_TUSS_ID = 4486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4488 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.887' WHERE PROPTB_TUSS_ID = 4487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4489 , CLOUD_SYNC_DATE = '2014-05-06 17:39:16.987' WHERE PROPTB_TUSS_ID = 4488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4490 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.087' WHERE PROPTB_TUSS_ID = 4489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4491 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.193' WHERE PROPTB_TUSS_ID = 4490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4492 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.293' WHERE PROPTB_TUSS_ID = 4491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4493 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.390' WHERE PROPTB_TUSS_ID = 4492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4494 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.497' WHERE PROPTB_TUSS_ID = 4493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4495 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.597' WHERE PROPTB_TUSS_ID = 4494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4496 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.703' WHERE PROPTB_TUSS_ID = 4495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4497 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.803' WHERE PROPTB_TUSS_ID = 4496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4498 , CLOUD_SYNC_DATE = '2014-05-06 17:39:17.907' WHERE PROPTB_TUSS_ID = 4497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4499 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.010' WHERE PROPTB_TUSS_ID = 4498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4500 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.110' WHERE PROPTB_TUSS_ID = 4499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4501 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.213' WHERE PROPTB_TUSS_ID = 4500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4502 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.317' WHERE PROPTB_TUSS_ID = 4501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4503 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.417' WHERE PROPTB_TUSS_ID = 4502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4504 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.523' WHERE PROPTB_TUSS_ID = 4503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4505 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.627' WHERE PROPTB_TUSS_ID = 4504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4506 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.733' WHERE PROPTB_TUSS_ID = 4505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4507 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.830' WHERE PROPTB_TUSS_ID = 4506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4508 , CLOUD_SYNC_DATE = '2014-05-06 17:39:18.933' WHERE PROPTB_TUSS_ID = 4507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4509 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.037' WHERE PROPTB_TUSS_ID = 4508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4510 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.137' WHERE PROPTB_TUSS_ID = 4509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4511 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.240' WHERE PROPTB_TUSS_ID = 4510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4512 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.343' WHERE PROPTB_TUSS_ID = 4511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4513 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.443' WHERE PROPTB_TUSS_ID = 4512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4514 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.543' WHERE PROPTB_TUSS_ID = 4513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4515 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.647' WHERE PROPTB_TUSS_ID = 4514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4516 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.750' WHERE PROPTB_TUSS_ID = 4515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4517 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.853' WHERE PROPTB_TUSS_ID = 4516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4518 , CLOUD_SYNC_DATE = '2014-05-06 17:39:19.957' WHERE PROPTB_TUSS_ID = 4517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4519 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.063' WHERE PROPTB_TUSS_ID = 4518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4520 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.167' WHERE PROPTB_TUSS_ID = 4519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4521 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.270' WHERE PROPTB_TUSS_ID = 4520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4522 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.367' WHERE PROPTB_TUSS_ID = 4521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4523 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.470' WHERE PROPTB_TUSS_ID = 4522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4524 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.577' WHERE PROPTB_TUSS_ID = 4523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4525 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.680' WHERE PROPTB_TUSS_ID = 4524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4526 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.783' WHERE PROPTB_TUSS_ID = 4525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4527 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.887' WHERE PROPTB_TUSS_ID = 4526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4528 , CLOUD_SYNC_DATE = '2014-05-06 17:39:20.990' WHERE PROPTB_TUSS_ID = 4527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4529 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.093' WHERE PROPTB_TUSS_ID = 4528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4530 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.197' WHERE PROPTB_TUSS_ID = 4529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4531 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.300' WHERE PROPTB_TUSS_ID = 4530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4532 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.403' WHERE PROPTB_TUSS_ID = 4531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4533 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.507' WHERE PROPTB_TUSS_ID = 4532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4534 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.617' WHERE PROPTB_TUSS_ID = 4533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4535 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.723' WHERE PROPTB_TUSS_ID = 4534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4536 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.827' WHERE PROPTB_TUSS_ID = 4535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4537 , CLOUD_SYNC_DATE = '2014-05-06 17:39:21.933' WHERE PROPTB_TUSS_ID = 4536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4538 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.037' WHERE PROPTB_TUSS_ID = 4537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4539 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.137' WHERE PROPTB_TUSS_ID = 4538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4540 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.240' WHERE PROPTB_TUSS_ID = 4539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4541 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.343' WHERE PROPTB_TUSS_ID = 4540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4542 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.447' WHERE PROPTB_TUSS_ID = 4541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4543 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.550' WHERE PROPTB_TUSS_ID = 4542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4544 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.653' WHERE PROPTB_TUSS_ID = 4543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4545 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.760' WHERE PROPTB_TUSS_ID = 4544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4546 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.863' WHERE PROPTB_TUSS_ID = 4545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4547 , CLOUD_SYNC_DATE = '2014-05-06 17:39:22.967' WHERE PROPTB_TUSS_ID = 4546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4548 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.067' WHERE PROPTB_TUSS_ID = 4547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4549 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.170' WHERE PROPTB_TUSS_ID = 4548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4550 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.280' WHERE PROPTB_TUSS_ID = 4549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4551 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.387' WHERE PROPTB_TUSS_ID = 4550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4552 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.493' WHERE PROPTB_TUSS_ID = 4551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4553 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.597' WHERE PROPTB_TUSS_ID = 4552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4554 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.710' WHERE PROPTB_TUSS_ID = 4553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4555 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.813' WHERE PROPTB_TUSS_ID = 4554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4556 , CLOUD_SYNC_DATE = '2014-05-06 17:39:23.917' WHERE PROPTB_TUSS_ID = 4555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4557 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.020' WHERE PROPTB_TUSS_ID = 4556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4558 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.127' WHERE PROPTB_TUSS_ID = 4557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4559 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.227' WHERE PROPTB_TUSS_ID = 4558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4560 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.327' WHERE PROPTB_TUSS_ID = 4559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4561 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.430' WHERE PROPTB_TUSS_ID = 4560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4562 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.537' WHERE PROPTB_TUSS_ID = 4561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4563 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.640' WHERE PROPTB_TUSS_ID = 4562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4564 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.743' WHERE PROPTB_TUSS_ID = 4563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4565 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.850' WHERE PROPTB_TUSS_ID = 4564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4566 , CLOUD_SYNC_DATE = '2014-05-06 17:39:24.953' WHERE PROPTB_TUSS_ID = 4565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4567 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.057' WHERE PROPTB_TUSS_ID = 4566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4568 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.160' WHERE PROPTB_TUSS_ID = 4567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4569 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.263' WHERE PROPTB_TUSS_ID = 4568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4570 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.367' WHERE PROPTB_TUSS_ID = 4569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4571 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.463' WHERE PROPTB_TUSS_ID = 4570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4572 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.567' WHERE PROPTB_TUSS_ID = 4571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4573 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.673' WHERE PROPTB_TUSS_ID = 4572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4574 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.777' WHERE PROPTB_TUSS_ID = 4573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4575 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.877' WHERE PROPTB_TUSS_ID = 4574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4576 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.983' WHERE PROPTB_TUSS_ID = 4575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4577 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.090' WHERE PROPTB_TUSS_ID = 4576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4578 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.193' WHERE PROPTB_TUSS_ID = 4577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4579 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.297' WHERE PROPTB_TUSS_ID = 4578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4580 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.400' WHERE PROPTB_TUSS_ID = 4579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4581 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.503' WHERE PROPTB_TUSS_ID = 4580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4582 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.607' WHERE PROPTB_TUSS_ID = 4581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4583 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.713' WHERE PROPTB_TUSS_ID = 4582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4584 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.817' WHERE PROPTB_TUSS_ID = 4583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4585 , CLOUD_SYNC_DATE = '2014-05-06 17:39:26.923' WHERE PROPTB_TUSS_ID = 4584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4586 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.027' WHERE PROPTB_TUSS_ID = 4585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4587 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.127' WHERE PROPTB_TUSS_ID = 4586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4588 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.230' WHERE PROPTB_TUSS_ID = 4587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4589 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.337' WHERE PROPTB_TUSS_ID = 4588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4590 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.440' WHERE PROPTB_TUSS_ID = 4589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4591 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.547' WHERE PROPTB_TUSS_ID = 4590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4592 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.653' WHERE PROPTB_TUSS_ID = 4591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4593 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.760' WHERE PROPTB_TUSS_ID = 4592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4594 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.863' WHERE PROPTB_TUSS_ID = 4593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4595 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.967' WHERE PROPTB_TUSS_ID = 4594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4596 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.077' WHERE PROPTB_TUSS_ID = 4595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4597 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.180' WHERE PROPTB_TUSS_ID = 4596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4598 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.283' WHERE PROPTB_TUSS_ID = 4597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4599 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.390' WHERE PROPTB_TUSS_ID = 4598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4600 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.497' WHERE PROPTB_TUSS_ID = 4599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4601 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.600' WHERE PROPTB_TUSS_ID = 4600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4602 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.710' WHERE PROPTB_TUSS_ID = 4601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4603 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.813' WHERE PROPTB_TUSS_ID = 4602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4604 , CLOUD_SYNC_DATE = '2014-05-06 17:39:28.920' WHERE PROPTB_TUSS_ID = 4603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4605 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.023' WHERE PROPTB_TUSS_ID = 4604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4606 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.127' WHERE PROPTB_TUSS_ID = 4605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4607 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.233' WHERE PROPTB_TUSS_ID = 4606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4608 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.337' WHERE PROPTB_TUSS_ID = 4607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4609 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.440' WHERE PROPTB_TUSS_ID = 4608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4610 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.547' WHERE PROPTB_TUSS_ID = 4609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4611 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.657' WHERE PROPTB_TUSS_ID = 4610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4612 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.767' WHERE PROPTB_TUSS_ID = 4611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4613 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.870' WHERE PROPTB_TUSS_ID = 4612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4614 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.973' WHERE PROPTB_TUSS_ID = 4613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4615 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.080' WHERE PROPTB_TUSS_ID = 4614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4616 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.183' WHERE PROPTB_TUSS_ID = 4615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4617 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.287' WHERE PROPTB_TUSS_ID = 4616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4618 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.393' WHERE PROPTB_TUSS_ID = 4617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4619 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.497' WHERE PROPTB_TUSS_ID = 4618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4620 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.600' WHERE PROPTB_TUSS_ID = 4619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4621 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.710' WHERE PROPTB_TUSS_ID = 4620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4622 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.817' WHERE PROPTB_TUSS_ID = 4621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4623 , CLOUD_SYNC_DATE = '2014-05-06 17:39:30.920' WHERE PROPTB_TUSS_ID = 4622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4624 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.027' WHERE PROPTB_TUSS_ID = 4623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4625 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.137' WHERE PROPTB_TUSS_ID = 4624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4626 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.240' WHERE PROPTB_TUSS_ID = 4625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4627 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.343' WHERE PROPTB_TUSS_ID = 4626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4628 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.447' WHERE PROPTB_TUSS_ID = 4627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4629 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.553' WHERE PROPTB_TUSS_ID = 4628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4630 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.660' WHERE PROPTB_TUSS_ID = 4629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4631 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.767' WHERE PROPTB_TUSS_ID = 4630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4632 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.870' WHERE PROPTB_TUSS_ID = 4631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4633 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.977' WHERE PROPTB_TUSS_ID = 4632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4634 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.080' WHERE PROPTB_TUSS_ID = 4633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4635 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.187' WHERE PROPTB_TUSS_ID = 4634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4636 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.290' WHERE PROPTB_TUSS_ID = 4635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4637 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.393' WHERE PROPTB_TUSS_ID = 4636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4638 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.500' WHERE PROPTB_TUSS_ID = 4637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4639 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.607' WHERE PROPTB_TUSS_ID = 4638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4640 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.713' WHERE PROPTB_TUSS_ID = 4639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4641 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.820' WHERE PROPTB_TUSS_ID = 4640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4642 , CLOUD_SYNC_DATE = '2014-05-06 17:39:32.927' WHERE PROPTB_TUSS_ID = 4641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4643 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.033' WHERE PROPTB_TUSS_ID = 4642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4644 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.140' WHERE PROPTB_TUSS_ID = 4643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4645 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.247' WHERE PROPTB_TUSS_ID = 4644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4646 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.370' WHERE PROPTB_TUSS_ID = 4645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4647 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.507' WHERE PROPTB_TUSS_ID = 4646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4648 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.627' WHERE PROPTB_TUSS_ID = 4647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4649 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.740' WHERE PROPTB_TUSS_ID = 4648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4650 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.843' WHERE PROPTB_TUSS_ID = 4649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4651 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.953' WHERE PROPTB_TUSS_ID = 4650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4652 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.057' WHERE PROPTB_TUSS_ID = 4651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4653 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.163' WHERE PROPTB_TUSS_ID = 4652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4654 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.267' WHERE PROPTB_TUSS_ID = 4653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4655 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.370' WHERE PROPTB_TUSS_ID = 4654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4656 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.483' WHERE PROPTB_TUSS_ID = 4655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4657 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.607' WHERE PROPTB_TUSS_ID = 4656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4658 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.723' WHERE PROPTB_TUSS_ID = 4657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4659 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.827' WHERE PROPTB_TUSS_ID = 4658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4660 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.933' WHERE PROPTB_TUSS_ID = 4659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4661 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.040' WHERE PROPTB_TUSS_ID = 4660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4662 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.143' WHERE PROPTB_TUSS_ID = 4661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4663 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.250' WHERE PROPTB_TUSS_ID = 4662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4664 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.353' WHERE PROPTB_TUSS_ID = 4663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4665 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.463' WHERE PROPTB_TUSS_ID = 4664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4666 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.603' WHERE PROPTB_TUSS_ID = 4665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4667 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.737' WHERE PROPTB_TUSS_ID = 4666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4668 , CLOUD_SYNC_DATE = '2014-05-06 17:39:35.860' WHERE PROPTB_TUSS_ID = 4667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4669 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.003' WHERE PROPTB_TUSS_ID = 4668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4670 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.133' WHERE PROPTB_TUSS_ID = 4669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4671 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.303' WHERE PROPTB_TUSS_ID = 4670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4672 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.420' WHERE PROPTB_TUSS_ID = 4671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4673 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.527' WHERE PROPTB_TUSS_ID = 4672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4674 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.633' WHERE PROPTB_TUSS_ID = 4673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4675 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.747' WHERE PROPTB_TUSS_ID = 4674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4676 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.850' WHERE PROPTB_TUSS_ID = 4675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4677 , CLOUD_SYNC_DATE = '2014-05-06 17:39:36.957' WHERE PROPTB_TUSS_ID = 4676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4678 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.063' WHERE PROPTB_TUSS_ID = 4677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4679 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.170' WHERE PROPTB_TUSS_ID = 4678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4680 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.277' WHERE PROPTB_TUSS_ID = 4679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4681 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.383' WHERE PROPTB_TUSS_ID = 4680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4682 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.490' WHERE PROPTB_TUSS_ID = 4681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4683 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.597' WHERE PROPTB_TUSS_ID = 4682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4684 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.707' WHERE PROPTB_TUSS_ID = 4683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4685 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.810' WHERE PROPTB_TUSS_ID = 4684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4686 , CLOUD_SYNC_DATE = '2014-05-06 17:39:37.917' WHERE PROPTB_TUSS_ID = 4685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4687 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.020' WHERE PROPTB_TUSS_ID = 4686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4688 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.127' WHERE PROPTB_TUSS_ID = 4687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4689 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.233' WHERE PROPTB_TUSS_ID = 4688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4690 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.340' WHERE PROPTB_TUSS_ID = 4689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4691 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.443' WHERE PROPTB_TUSS_ID = 4690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4692 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.550' WHERE PROPTB_TUSS_ID = 4691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4693 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.660' WHERE PROPTB_TUSS_ID = 4692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4694 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.767' WHERE PROPTB_TUSS_ID = 4693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4695 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.870' WHERE PROPTB_TUSS_ID = 4694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4696 , CLOUD_SYNC_DATE = '2014-05-06 17:39:38.977' WHERE PROPTB_TUSS_ID = 4695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4697 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.090' WHERE PROPTB_TUSS_ID = 4696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4698 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.207' WHERE PROPTB_TUSS_ID = 4697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4699 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.313' WHERE PROPTB_TUSS_ID = 4698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4700 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.417' WHERE PROPTB_TUSS_ID = 4699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4701 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.523' WHERE PROPTB_TUSS_ID = 4700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4702 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.633' WHERE PROPTB_TUSS_ID = 4701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4703 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.743' WHERE PROPTB_TUSS_ID = 4702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4704 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.850' WHERE PROPTB_TUSS_ID = 4703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4705 , CLOUD_SYNC_DATE = '2014-05-06 17:39:39.957' WHERE PROPTB_TUSS_ID = 4704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4706 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.063' WHERE PROPTB_TUSS_ID = 4705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4707 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.167' WHERE PROPTB_TUSS_ID = 4706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4708 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.277' WHERE PROPTB_TUSS_ID = 4707
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4709 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.383' WHERE PROPTB_TUSS_ID = 4708
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4710 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.487' WHERE PROPTB_TUSS_ID = 4709
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4711 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.603' WHERE PROPTB_TUSS_ID = 4710
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4712 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.713' WHERE PROPTB_TUSS_ID = 4711
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4713 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.823' WHERE PROPTB_TUSS_ID = 4712
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4714 , CLOUD_SYNC_DATE = '2014-05-06 17:39:40.933' WHERE PROPTB_TUSS_ID = 4713
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4715 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.037' WHERE PROPTB_TUSS_ID = 4714
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4716 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.143' WHERE PROPTB_TUSS_ID = 4715
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4717 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.253' WHERE PROPTB_TUSS_ID = 4716
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4718 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.360' WHERE PROPTB_TUSS_ID = 4717
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4719 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.467' WHERE PROPTB_TUSS_ID = 4718
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4720 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.573' WHERE PROPTB_TUSS_ID = 4719
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4721 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.697' WHERE PROPTB_TUSS_ID = 4720
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4722 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.803' WHERE PROPTB_TUSS_ID = 4721
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4723 , CLOUD_SYNC_DATE = '2014-05-06 17:39:41.913' WHERE PROPTB_TUSS_ID = 4722
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4724 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.020' WHERE PROPTB_TUSS_ID = 4723
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4725 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.127' WHERE PROPTB_TUSS_ID = 4724
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4726 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.233' WHERE PROPTB_TUSS_ID = 4725
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4727 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.337' WHERE PROPTB_TUSS_ID = 4726
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4728 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.447' WHERE PROPTB_TUSS_ID = 4727
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4729 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.550' WHERE PROPTB_TUSS_ID = 4728
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4730 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.663' WHERE PROPTB_TUSS_ID = 4729
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4731 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.773' WHERE PROPTB_TUSS_ID = 4730
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4732 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.880' WHERE PROPTB_TUSS_ID = 4731
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4733 , CLOUD_SYNC_DATE = '2014-05-06 17:39:42.993' WHERE PROPTB_TUSS_ID = 4732
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4734 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.100' WHERE PROPTB_TUSS_ID = 4733
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4735 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.207' WHERE PROPTB_TUSS_ID = 4734
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4736 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.317' WHERE PROPTB_TUSS_ID = 4735
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4737 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.430' WHERE PROPTB_TUSS_ID = 4736
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4738 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.547' WHERE PROPTB_TUSS_ID = 4737
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4739 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.667' WHERE PROPTB_TUSS_ID = 4738
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4740 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.780' WHERE PROPTB_TUSS_ID = 4739
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4741 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.890' WHERE PROPTB_TUSS_ID = 4740
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4742 , CLOUD_SYNC_DATE = '2014-05-06 17:39:43.997' WHERE PROPTB_TUSS_ID = 4741
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4743 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.103' WHERE PROPTB_TUSS_ID = 4742
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4744 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.207' WHERE PROPTB_TUSS_ID = 4743
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4745 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.313' WHERE PROPTB_TUSS_ID = 4744
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4746 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.420' WHERE PROPTB_TUSS_ID = 4745
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4747 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.527' WHERE PROPTB_TUSS_ID = 4746
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4748 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.643' WHERE PROPTB_TUSS_ID = 4747
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4749 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.757' WHERE PROPTB_TUSS_ID = 4748
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4750 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.863' WHERE PROPTB_TUSS_ID = 4749
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4751 , CLOUD_SYNC_DATE = '2014-05-06 17:39:44.970' WHERE PROPTB_TUSS_ID = 4750
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4752 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.077' WHERE PROPTB_TUSS_ID = 4751
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4753 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.183' WHERE PROPTB_TUSS_ID = 4752
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4754 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.290' WHERE PROPTB_TUSS_ID = 4753
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4755 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.397' WHERE PROPTB_TUSS_ID = 4754
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4756 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.503' WHERE PROPTB_TUSS_ID = 4755
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4757 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.617' WHERE PROPTB_TUSS_ID = 4756
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4758 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.727' WHERE PROPTB_TUSS_ID = 4757
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4759 , CLOUD_SYNC_DATE = '2014-05-06 17:39:45.833' WHERE PROPTB_TUSS_ID = 4758
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4016 , CLOUD_SYNC_DATE = '2014-05-06 17:38:30.807' WHERE PROPTB_TUSS_ID = 4759
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4761 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.050' WHERE PROPTB_TUSS_ID = 4760
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4762 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.157' WHERE PROPTB_TUSS_ID = 4761
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4763 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.263' WHERE PROPTB_TUSS_ID = 4762
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4764 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.373' WHERE PROPTB_TUSS_ID = 4763
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4765 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.483' WHERE PROPTB_TUSS_ID = 4764
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4766 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.613' WHERE PROPTB_TUSS_ID = 4765
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4767 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.757' WHERE PROPTB_TUSS_ID = 4766
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4768 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.863' WHERE PROPTB_TUSS_ID = 4767
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4769 , CLOUD_SYNC_DATE = '2014-05-06 17:39:46.970' WHERE PROPTB_TUSS_ID = 4768
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4770 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.080' WHERE PROPTB_TUSS_ID = 4769
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4771 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.190' WHERE PROPTB_TUSS_ID = 4770
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4772 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.317' WHERE PROPTB_TUSS_ID = 4771
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4773 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.433' WHERE PROPTB_TUSS_ID = 4772
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4774 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.543' WHERE PROPTB_TUSS_ID = 4773
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4775 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.653' WHERE PROPTB_TUSS_ID = 4774
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4776 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.763' WHERE PROPTB_TUSS_ID = 4775
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4777 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.873' WHERE PROPTB_TUSS_ID = 4776
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4778 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.993' WHERE PROPTB_TUSS_ID = 4777
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4779 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.117' WHERE PROPTB_TUSS_ID = 4778
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4780 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.227' WHERE PROPTB_TUSS_ID = 4779
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4781 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.333' WHERE PROPTB_TUSS_ID = 4780
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4782 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.447' WHERE PROPTB_TUSS_ID = 4781
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4783 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.553' WHERE PROPTB_TUSS_ID = 4782
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4784 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.670' WHERE PROPTB_TUSS_ID = 4783
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4785 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.783' WHERE PROPTB_TUSS_ID = 4784
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4786 , CLOUD_SYNC_DATE = '2014-05-06 17:39:48.893' WHERE PROPTB_TUSS_ID = 4785
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4787 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.003' WHERE PROPTB_TUSS_ID = 4786
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4788 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.110' WHERE PROPTB_TUSS_ID = 4787
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4789 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.223' WHERE PROPTB_TUSS_ID = 4788
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4790 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.340' WHERE PROPTB_TUSS_ID = 4789
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4791 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.447' WHERE PROPTB_TUSS_ID = 4790
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4792 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.557' WHERE PROPTB_TUSS_ID = 4791
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4793 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.683' WHERE PROPTB_TUSS_ID = 4792
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4794 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.797' WHERE PROPTB_TUSS_ID = 4793
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4795 , CLOUD_SYNC_DATE = '2014-05-06 17:39:49.913' WHERE PROPTB_TUSS_ID = 4794
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4796 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.030' WHERE PROPTB_TUSS_ID = 4795
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4797 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.150' WHERE PROPTB_TUSS_ID = 4796
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4798 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.263' WHERE PROPTB_TUSS_ID = 4797
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4799 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.373' WHERE PROPTB_TUSS_ID = 4798
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4800 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.487' WHERE PROPTB_TUSS_ID = 4799
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4801 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.600' WHERE PROPTB_TUSS_ID = 4800
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4802 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.713' WHERE PROPTB_TUSS_ID = 4801
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4803 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.823' WHERE PROPTB_TUSS_ID = 4802
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4804 , CLOUD_SYNC_DATE = '2014-05-06 17:39:50.933' WHERE PROPTB_TUSS_ID = 4803
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4805 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.040' WHERE PROPTB_TUSS_ID = 4804
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4806 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.150' WHERE PROPTB_TUSS_ID = 4805
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4807 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.263' WHERE PROPTB_TUSS_ID = 4806
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4808 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.373' WHERE PROPTB_TUSS_ID = 4807
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4809 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.483' WHERE PROPTB_TUSS_ID = 4808
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4810 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.593' WHERE PROPTB_TUSS_ID = 4809
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4811 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.707' WHERE PROPTB_TUSS_ID = 4810
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4812 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.817' WHERE PROPTB_TUSS_ID = 4811
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4813 , CLOUD_SYNC_DATE = '2014-05-06 17:39:51.927' WHERE PROPTB_TUSS_ID = 4812
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4814 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.037' WHERE PROPTB_TUSS_ID = 4813
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4815 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.143' WHERE PROPTB_TUSS_ID = 4814
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4816 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.250' WHERE PROPTB_TUSS_ID = 4815
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4817 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.360' WHERE PROPTB_TUSS_ID = 4816
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4818 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.470' WHERE PROPTB_TUSS_ID = 4817
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4819 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.577' WHERE PROPTB_TUSS_ID = 4818
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4820 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.693' WHERE PROPTB_TUSS_ID = 4819
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4821 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.803' WHERE PROPTB_TUSS_ID = 4820
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4822 , CLOUD_SYNC_DATE = '2014-05-06 17:39:52.913' WHERE PROPTB_TUSS_ID = 4821
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4823 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.023' WHERE PROPTB_TUSS_ID = 4822
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4824 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.133' WHERE PROPTB_TUSS_ID = 4823
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4825 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.240' WHERE PROPTB_TUSS_ID = 4824
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4826 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.347' WHERE PROPTB_TUSS_ID = 4825
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4827 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.457' WHERE PROPTB_TUSS_ID = 4826
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4828 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.580' WHERE PROPTB_TUSS_ID = 4827
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4829 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.703' WHERE PROPTB_TUSS_ID = 4828
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4830 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.810' WHERE PROPTB_TUSS_ID = 4829
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4831 , CLOUD_SYNC_DATE = '2014-05-06 17:39:53.920' WHERE PROPTB_TUSS_ID = 4830
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4832 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.033' WHERE PROPTB_TUSS_ID = 4831
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4833 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.143' WHERE PROPTB_TUSS_ID = 4832
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4834 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.250' WHERE PROPTB_TUSS_ID = 4833
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4835 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.360' WHERE PROPTB_TUSS_ID = 4834
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4836 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.467' WHERE PROPTB_TUSS_ID = 4835
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4837 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.583' WHERE PROPTB_TUSS_ID = 4836
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4838 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.697' WHERE PROPTB_TUSS_ID = 4837
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4839 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.807' WHERE PROPTB_TUSS_ID = 4838
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4840 , CLOUD_SYNC_DATE = '2014-05-06 17:39:54.917' WHERE PROPTB_TUSS_ID = 4839
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4380 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.907' WHERE PROPTB_TUSS_ID = 4840
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4325 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.470' WHERE PROPTB_TUSS_ID = 4841
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4328 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.780' WHERE PROPTB_TUSS_ID = 4842
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4844 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.367' WHERE PROPTB_TUSS_ID = 4843
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4845 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.480' WHERE PROPTB_TUSS_ID = 4844
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4846 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.593' WHERE PROPTB_TUSS_ID = 4845
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4847 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.707' WHERE PROPTB_TUSS_ID = 4846
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4848 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.817' WHERE PROPTB_TUSS_ID = 4847
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4849 , CLOUD_SYNC_DATE = '2014-05-06 17:39:55.923' WHERE PROPTB_TUSS_ID = 4848
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4850 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.033' WHERE PROPTB_TUSS_ID = 4849
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4851 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.143' WHERE PROPTB_TUSS_ID = 4850
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4852 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.253' WHERE PROPTB_TUSS_ID = 4851
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4853 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.367' WHERE PROPTB_TUSS_ID = 4852
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4854 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.477' WHERE PROPTB_TUSS_ID = 4853
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4855 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.587' WHERE PROPTB_TUSS_ID = 4854
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4856 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.700' WHERE PROPTB_TUSS_ID = 4855
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4857 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.810' WHERE PROPTB_TUSS_ID = 4856
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4858 , CLOUD_SYNC_DATE = '2014-05-06 17:39:56.923' WHERE PROPTB_TUSS_ID = 4857
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4859 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.033' WHERE PROPTB_TUSS_ID = 4858
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4860 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.140' WHERE PROPTB_TUSS_ID = 4859
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4861 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.250' WHERE PROPTB_TUSS_ID = 4860
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4862 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.367' WHERE PROPTB_TUSS_ID = 4861
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4863 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.473' WHERE PROPTB_TUSS_ID = 4862
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4864 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.587' WHERE PROPTB_TUSS_ID = 4863
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4865 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.700' WHERE PROPTB_TUSS_ID = 4864
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4866 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.817' WHERE PROPTB_TUSS_ID = 4865
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4867 , CLOUD_SYNC_DATE = '2014-05-06 17:39:57.923' WHERE PROPTB_TUSS_ID = 4866
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4868 , CLOUD_SYNC_DATE = '2014-05-06 17:39:58.033' WHERE PROPTB_TUSS_ID = 4867
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4869 , CLOUD_SYNC_DATE = '2014-05-06 17:39:58.143' WHERE PROPTB_TUSS_ID = 4868
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4591 , CLOUD_SYNC_DATE = '2014-05-06 17:39:27.547' WHERE PROPTB_TUSS_ID = 4869
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4608 , CLOUD_SYNC_DATE = '2014-05-06 17:39:29.337' WHERE PROPTB_TUSS_ID = 4870
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4629 , CLOUD_SYNC_DATE = '2014-05-06 17:39:31.553' WHERE PROPTB_TUSS_ID = 4871
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4645 , CLOUD_SYNC_DATE = '2014-05-06 17:39:33.247' WHERE PROPTB_TUSS_ID = 4872
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4652 , CLOUD_SYNC_DATE = '2014-05-06 17:39:34.057' WHERE PROPTB_TUSS_ID = 4873
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4875 , CLOUD_SYNC_DATE = '2014-05-06 17:39:58.813' WHERE PROPTB_TUSS_ID = 4874
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4876 , CLOUD_SYNC_DATE = '2014-05-06 17:39:58.923' WHERE PROPTB_TUSS_ID = 4875
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4877 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.030' WHERE PROPTB_TUSS_ID = 4876
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4878 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.147' WHERE PROPTB_TUSS_ID = 4877
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4879 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.257' WHERE PROPTB_TUSS_ID = 4878
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4880 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.370' WHERE PROPTB_TUSS_ID = 4879
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4881 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.483' WHERE PROPTB_TUSS_ID = 4880
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4882 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.597' WHERE PROPTB_TUSS_ID = 4881
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4883 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.710' WHERE PROPTB_TUSS_ID = 4882
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4884 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.817' WHERE PROPTB_TUSS_ID = 4883
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4885 , CLOUD_SYNC_DATE = '2014-05-06 17:39:59.933' WHERE PROPTB_TUSS_ID = 4884
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4886 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.043' WHERE PROPTB_TUSS_ID = 4885
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4887 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.153' WHERE PROPTB_TUSS_ID = 4886
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4888 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.260' WHERE PROPTB_TUSS_ID = 4887
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4889 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.377' WHERE PROPTB_TUSS_ID = 4888
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4890 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.487' WHERE PROPTB_TUSS_ID = 4889
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4891 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.593' WHERE PROPTB_TUSS_ID = 4890
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4892 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.707' WHERE PROPTB_TUSS_ID = 4891
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4893 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.820' WHERE PROPTB_TUSS_ID = 4892
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4894 , CLOUD_SYNC_DATE = '2014-05-06 17:40:00.933' WHERE PROPTB_TUSS_ID = 4893
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4895 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.043' WHERE PROPTB_TUSS_ID = 4894
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4896 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.157' WHERE PROPTB_TUSS_ID = 4895
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4897 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.267' WHERE PROPTB_TUSS_ID = 4896
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4898 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.377' WHERE PROPTB_TUSS_ID = 4897
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4899 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.487' WHERE PROPTB_TUSS_ID = 4898
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4900 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.600' WHERE PROPTB_TUSS_ID = 4899
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4901 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.713' WHERE PROPTB_TUSS_ID = 4900
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4902 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.827' WHERE PROPTB_TUSS_ID = 4901
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4903 , CLOUD_SYNC_DATE = '2014-05-06 17:40:01.940' WHERE PROPTB_TUSS_ID = 4902
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4904 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.050' WHERE PROPTB_TUSS_ID = 4903
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4905 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.167' WHERE PROPTB_TUSS_ID = 4904
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4906 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.290' WHERE PROPTB_TUSS_ID = 4905
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4907 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.417' WHERE PROPTB_TUSS_ID = 4906
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4908 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.537' WHERE PROPTB_TUSS_ID = 4907
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4909 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.647' WHERE PROPTB_TUSS_ID = 4908
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4910 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.767' WHERE PROPTB_TUSS_ID = 4909
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4911 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.877' WHERE PROPTB_TUSS_ID = 4910
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4912 , CLOUD_SYNC_DATE = '2014-05-06 17:40:02.987' WHERE PROPTB_TUSS_ID = 4911
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4913 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.100' WHERE PROPTB_TUSS_ID = 4912
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4914 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.210' WHERE PROPTB_TUSS_ID = 4913
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4915 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.320' WHERE PROPTB_TUSS_ID = 4914
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4916 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.433' WHERE PROPTB_TUSS_ID = 4915
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4917 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.550' WHERE PROPTB_TUSS_ID = 4916
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4918 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.680' WHERE PROPTB_TUSS_ID = 4917
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4919 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.797' WHERE PROPTB_TUSS_ID = 4918
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4920 , CLOUD_SYNC_DATE = '2014-05-06 17:40:03.910' WHERE PROPTB_TUSS_ID = 4919
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4921 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.020' WHERE PROPTB_TUSS_ID = 4920
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4922 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.133' WHERE PROPTB_TUSS_ID = 4921
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4923 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.250' WHERE PROPTB_TUSS_ID = 4922
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4924 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.360' WHERE PROPTB_TUSS_ID = 4923
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4925 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.473' WHERE PROPTB_TUSS_ID = 4924
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4926 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.593' WHERE PROPTB_TUSS_ID = 4925
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4927 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.707' WHERE PROPTB_TUSS_ID = 4926
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4928 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.817' WHERE PROPTB_TUSS_ID = 4927
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4929 , CLOUD_SYNC_DATE = '2014-05-06 17:40:04.943' WHERE PROPTB_TUSS_ID = 4928
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4930 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.060' WHERE PROPTB_TUSS_ID = 4929
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4931 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.173' WHERE PROPTB_TUSS_ID = 4930
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4932 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.290' WHERE PROPTB_TUSS_ID = 4931
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4933 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.407' WHERE PROPTB_TUSS_ID = 4932
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4934 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.527' WHERE PROPTB_TUSS_ID = 4933
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4935 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.637' WHERE PROPTB_TUSS_ID = 4934
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4936 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.757' WHERE PROPTB_TUSS_ID = 4935
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4937 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.867' WHERE PROPTB_TUSS_ID = 4936
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4938 , CLOUD_SYNC_DATE = '2014-05-06 17:40:05.980' WHERE PROPTB_TUSS_ID = 4937
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4939 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.093' WHERE PROPTB_TUSS_ID = 4938
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4940 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.207' WHERE PROPTB_TUSS_ID = 4939
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4941 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.317' WHERE PROPTB_TUSS_ID = 4940
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4942 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.430' WHERE PROPTB_TUSS_ID = 4941
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4943 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.543' WHERE PROPTB_TUSS_ID = 4942
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4944 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.653' WHERE PROPTB_TUSS_ID = 4943
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4945 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.773' WHERE PROPTB_TUSS_ID = 4944
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4946 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.883' WHERE PROPTB_TUSS_ID = 4945
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4947 , CLOUD_SYNC_DATE = '2014-05-06 17:40:06.993' WHERE PROPTB_TUSS_ID = 4946
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4948 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.107' WHERE PROPTB_TUSS_ID = 4947
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4949 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.220' WHERE PROPTB_TUSS_ID = 4948
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4950 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.330' WHERE PROPTB_TUSS_ID = 4949
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4951 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.443' WHERE PROPTB_TUSS_ID = 4950
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4952 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.557' WHERE PROPTB_TUSS_ID = 4951
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4953 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.667' WHERE PROPTB_TUSS_ID = 4952
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4954 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.783' WHERE PROPTB_TUSS_ID = 4953
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4955 , CLOUD_SYNC_DATE = '2014-05-06 17:40:07.890' WHERE PROPTB_TUSS_ID = 4954
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4956 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.003' WHERE PROPTB_TUSS_ID = 4955
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4957 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.117' WHERE PROPTB_TUSS_ID = 4956
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4958 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.227' WHERE PROPTB_TUSS_ID = 4957
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4959 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.347' WHERE PROPTB_TUSS_ID = 4958
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4960 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.460' WHERE PROPTB_TUSS_ID = 4959
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4961 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.573' WHERE PROPTB_TUSS_ID = 4960
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4962 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.697' WHERE PROPTB_TUSS_ID = 4961
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4963 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.807' WHERE PROPTB_TUSS_ID = 4962
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4964 , CLOUD_SYNC_DATE = '2014-05-06 17:40:08.917' WHERE PROPTB_TUSS_ID = 4963
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4965 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.030' WHERE PROPTB_TUSS_ID = 4964
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4966 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.140' WHERE PROPTB_TUSS_ID = 4965
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4967 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.253' WHERE PROPTB_TUSS_ID = 4966
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4968 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.363' WHERE PROPTB_TUSS_ID = 4967
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4969 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.477' WHERE PROPTB_TUSS_ID = 4968
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4970 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.587' WHERE PROPTB_TUSS_ID = 4969
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4971 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.707' WHERE PROPTB_TUSS_ID = 4970
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4972 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.817' WHERE PROPTB_TUSS_ID = 4971
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4973 , CLOUD_SYNC_DATE = '2014-05-06 17:40:09.927' WHERE PROPTB_TUSS_ID = 4972
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4974 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.043' WHERE PROPTB_TUSS_ID = 4973
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4975 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.153' WHERE PROPTB_TUSS_ID = 4974
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4976 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.267' WHERE PROPTB_TUSS_ID = 4975
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4977 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.377' WHERE PROPTB_TUSS_ID = 4976
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4978 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.487' WHERE PROPTB_TUSS_ID = 4977
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4979 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.603' WHERE PROPTB_TUSS_ID = 4978
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4980 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.723' WHERE PROPTB_TUSS_ID = 4979
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4981 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.837' WHERE PROPTB_TUSS_ID = 4980
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4982 , CLOUD_SYNC_DATE = '2014-05-06 17:40:10.957' WHERE PROPTB_TUSS_ID = 4981
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4983 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.067' WHERE PROPTB_TUSS_ID = 4982
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4984 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.180' WHERE PROPTB_TUSS_ID = 4983
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4985 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.297' WHERE PROPTB_TUSS_ID = 4984
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4986 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.407' WHERE PROPTB_TUSS_ID = 4985
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4987 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.523' WHERE PROPTB_TUSS_ID = 4986
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4988 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.633' WHERE PROPTB_TUSS_ID = 4987
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4989 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.750' WHERE PROPTB_TUSS_ID = 4988
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4990 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.867' WHERE PROPTB_TUSS_ID = 4989
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4991 , CLOUD_SYNC_DATE = '2014-05-06 17:40:11.977' WHERE PROPTB_TUSS_ID = 4990
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4992 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.093' WHERE PROPTB_TUSS_ID = 4991
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4993 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.207' WHERE PROPTB_TUSS_ID = 4992
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4994 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.320' WHERE PROPTB_TUSS_ID = 4993
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4995 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.437' WHERE PROPTB_TUSS_ID = 4994
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4996 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.547' WHERE PROPTB_TUSS_ID = 4995
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4997 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.670' WHERE PROPTB_TUSS_ID = 4996
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4998 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.783' WHERE PROPTB_TUSS_ID = 4997
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4999 , CLOUD_SYNC_DATE = '2014-05-06 17:40:12.897' WHERE PROPTB_TUSS_ID = 4998
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5000 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.013' WHERE PROPTB_TUSS_ID = 4999
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5001 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.127' WHERE PROPTB_TUSS_ID = 5000
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5002 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.243' WHERE PROPTB_TUSS_ID = 5001
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5003 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.353' WHERE PROPTB_TUSS_ID = 5002
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5004 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.467' WHERE PROPTB_TUSS_ID = 5003
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5005 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.583' WHERE PROPTB_TUSS_ID = 5004
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5006 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.700' WHERE PROPTB_TUSS_ID = 5005
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5007 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.847' WHERE PROPTB_TUSS_ID = 5006
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5008 , CLOUD_SYNC_DATE = '2014-05-06 17:40:13.970' WHERE PROPTB_TUSS_ID = 5007
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5009 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.080' WHERE PROPTB_TUSS_ID = 5008
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5010 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.197' WHERE PROPTB_TUSS_ID = 5009
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5011 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.307' WHERE PROPTB_TUSS_ID = 5010
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5012 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.417' WHERE PROPTB_TUSS_ID = 5011
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5013 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.530' WHERE PROPTB_TUSS_ID = 5012
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5014 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.643' WHERE PROPTB_TUSS_ID = 5013
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5015 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.763' WHERE PROPTB_TUSS_ID = 5014
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5016 , CLOUD_SYNC_DATE = '2014-05-06 17:40:14.877' WHERE PROPTB_TUSS_ID = 5015
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5017 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.007' WHERE PROPTB_TUSS_ID = 5016
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5018 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.123' WHERE PROPTB_TUSS_ID = 5017
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5019 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.257' WHERE PROPTB_TUSS_ID = 5018
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5020 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.383' WHERE PROPTB_TUSS_ID = 5019
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5021 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.497' WHERE PROPTB_TUSS_ID = 5020
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5022 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.613' WHERE PROPTB_TUSS_ID = 5021
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5023 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.737' WHERE PROPTB_TUSS_ID = 5022
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5024 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.847' WHERE PROPTB_TUSS_ID = 5023
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5025 , CLOUD_SYNC_DATE = '2014-05-06 17:40:15.960' WHERE PROPTB_TUSS_ID = 5024
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5026 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.087' WHERE PROPTB_TUSS_ID = 5025
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5027 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.217' WHERE PROPTB_TUSS_ID = 5026
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5028 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.327' WHERE PROPTB_TUSS_ID = 5027
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5029 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.453' WHERE PROPTB_TUSS_ID = 5028
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5030 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.570' WHERE PROPTB_TUSS_ID = 5029
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5031 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.687' WHERE PROPTB_TUSS_ID = 5030
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5032 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.807' WHERE PROPTB_TUSS_ID = 5031
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5033 , CLOUD_SYNC_DATE = '2014-05-06 17:40:16.920' WHERE PROPTB_TUSS_ID = 5032
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5034 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.047' WHERE PROPTB_TUSS_ID = 5033
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5035 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.163' WHERE PROPTB_TUSS_ID = 5034
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5036 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.280' WHERE PROPTB_TUSS_ID = 5035
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5037 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.397' WHERE PROPTB_TUSS_ID = 5036
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5038 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.517' WHERE PROPTB_TUSS_ID = 5037
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5039 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.630' WHERE PROPTB_TUSS_ID = 5038
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5040 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.753' WHERE PROPTB_TUSS_ID = 5039
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5041 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.870' WHERE PROPTB_TUSS_ID = 5040
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5042 , CLOUD_SYNC_DATE = '2014-05-06 17:40:17.983' WHERE PROPTB_TUSS_ID = 5041
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5043 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.103' WHERE PROPTB_TUSS_ID = 5042
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5044 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.217' WHERE PROPTB_TUSS_ID = 5043
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5045 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.333' WHERE PROPTB_TUSS_ID = 5044
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5046 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.447' WHERE PROPTB_TUSS_ID = 5045
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5047 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.560' WHERE PROPTB_TUSS_ID = 5046
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5048 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.680' WHERE PROPTB_TUSS_ID = 5047
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5049 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.797' WHERE PROPTB_TUSS_ID = 5048
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5050 , CLOUD_SYNC_DATE = '2014-05-06 17:40:18.913' WHERE PROPTB_TUSS_ID = 5049
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5051 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.027' WHERE PROPTB_TUSS_ID = 5050
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5052 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.147' WHERE PROPTB_TUSS_ID = 5051
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5053 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.257' WHERE PROPTB_TUSS_ID = 5052
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5054 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.373' WHERE PROPTB_TUSS_ID = 5053
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5055 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.487' WHERE PROPTB_TUSS_ID = 5054
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5056 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.607' WHERE PROPTB_TUSS_ID = 5055
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5057 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.720' WHERE PROPTB_TUSS_ID = 5056
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5058 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.840' WHERE PROPTB_TUSS_ID = 5057
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5059 , CLOUD_SYNC_DATE = '2014-05-06 17:40:19.953' WHERE PROPTB_TUSS_ID = 5058
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5060 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.067' WHERE PROPTB_TUSS_ID = 5059
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5061 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.190' WHERE PROPTB_TUSS_ID = 5060
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5062 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.303' WHERE PROPTB_TUSS_ID = 5061
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5063 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.423' WHERE PROPTB_TUSS_ID = 5062
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4322 , CLOUD_SYNC_DATE = '2014-05-06 17:39:00.177' WHERE PROPTB_TUSS_ID = 5063
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4371 , CLOUD_SYNC_DATE = '2014-05-06 17:39:05.017' WHERE PROPTB_TUSS_ID = 5064
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5066 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.773' WHERE PROPTB_TUSS_ID = 5065
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5067 , CLOUD_SYNC_DATE = '2014-05-06 17:40:20.890' WHERE PROPTB_TUSS_ID = 5066
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5068 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.003' WHERE PROPTB_TUSS_ID = 5067
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5069 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.123' WHERE PROPTB_TUSS_ID = 5068
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5070 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.237' WHERE PROPTB_TUSS_ID = 5069
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5071 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.357' WHERE PROPTB_TUSS_ID = 5070
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5072 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.470' WHERE PROPTB_TUSS_ID = 5071
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5073 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.587' WHERE PROPTB_TUSS_ID = 5072
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5074 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.703' WHERE PROPTB_TUSS_ID = 5073
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5075 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.823' WHERE PROPTB_TUSS_ID = 5074
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5076 , CLOUD_SYNC_DATE = '2014-05-06 17:40:21.940' WHERE PROPTB_TUSS_ID = 5075
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5077 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.053' WHERE PROPTB_TUSS_ID = 5076
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5078 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.170' WHERE PROPTB_TUSS_ID = 5077
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5079 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.287' WHERE PROPTB_TUSS_ID = 5078
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5080 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.410' WHERE PROPTB_TUSS_ID = 5079
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5081 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.530' WHERE PROPTB_TUSS_ID = 5080
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5082 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.653' WHERE PROPTB_TUSS_ID = 5081
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5083 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.777' WHERE PROPTB_TUSS_ID = 5082
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5084 , CLOUD_SYNC_DATE = '2014-05-06 17:40:22.897' WHERE PROPTB_TUSS_ID = 5083
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5085 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.013' WHERE PROPTB_TUSS_ID = 5084
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5086 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.143' WHERE PROPTB_TUSS_ID = 5085
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5087 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.260' WHERE PROPTB_TUSS_ID = 5086
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5088 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.380' WHERE PROPTB_TUSS_ID = 5087
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5089 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.497' WHERE PROPTB_TUSS_ID = 5088
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5090 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.617' WHERE PROPTB_TUSS_ID = 5089
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5091 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.743' WHERE PROPTB_TUSS_ID = 5090
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5092 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.867' WHERE PROPTB_TUSS_ID = 5091
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5093 , CLOUD_SYNC_DATE = '2014-05-06 17:40:23.983' WHERE PROPTB_TUSS_ID = 5092
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5094 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.110' WHERE PROPTB_TUSS_ID = 5093
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 3323 , CLOUD_SYNC_DATE = '2014-05-06 17:37:31.810' WHERE PROPTB_TUSS_ID = 5094
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5096 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.387' WHERE PROPTB_TUSS_ID = 5095
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5097 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.503' WHERE PROPTB_TUSS_ID = 5096
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5098 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.623' WHERE PROPTB_TUSS_ID = 5097
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5099 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.743' WHERE PROPTB_TUSS_ID = 5098
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5100 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.860' WHERE PROPTB_TUSS_ID = 5099
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5101 , CLOUD_SYNC_DATE = '2014-05-06 17:40:24.977' WHERE PROPTB_TUSS_ID = 5100
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5102 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.097' WHERE PROPTB_TUSS_ID = 5101
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5103 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.210' WHERE PROPTB_TUSS_ID = 5102
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5104 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.330' WHERE PROPTB_TUSS_ID = 5103
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5105 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.443' WHERE PROPTB_TUSS_ID = 5104
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5106 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.563' WHERE PROPTB_TUSS_ID = 5105
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5107 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.677' WHERE PROPTB_TUSS_ID = 5106
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5108 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.817' WHERE PROPTB_TUSS_ID = 5107
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5109 , CLOUD_SYNC_DATE = '2014-05-06 17:40:25.937' WHERE PROPTB_TUSS_ID = 5108
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5110 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.057' WHERE PROPTB_TUSS_ID = 5109
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5111 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.173' WHERE PROPTB_TUSS_ID = 5110
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5112 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.293' WHERE PROPTB_TUSS_ID = 5111
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5113 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.407' WHERE PROPTB_TUSS_ID = 5112
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5114 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.527' WHERE PROPTB_TUSS_ID = 5113
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5115 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.653' WHERE PROPTB_TUSS_ID = 5114
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5116 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.777' WHERE PROPTB_TUSS_ID = 5115
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5117 , CLOUD_SYNC_DATE = '2014-05-06 17:40:26.893' WHERE PROPTB_TUSS_ID = 5116
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5118 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.010' WHERE PROPTB_TUSS_ID = 5117
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5119 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.167' WHERE PROPTB_TUSS_ID = 5118
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5120 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.287' WHERE PROPTB_TUSS_ID = 5119
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5121 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.403' WHERE PROPTB_TUSS_ID = 5120
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5122 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.520' WHERE PROPTB_TUSS_ID = 5121
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5123 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.637' WHERE PROPTB_TUSS_ID = 5122
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5124 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.773' WHERE PROPTB_TUSS_ID = 5123
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5125 , CLOUD_SYNC_DATE = '2014-05-06 17:40:27.900' WHERE PROPTB_TUSS_ID = 5124
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5126 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.017' WHERE PROPTB_TUSS_ID = 5125
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5127 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.140' WHERE PROPTB_TUSS_ID = 5126
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5128 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.257' WHERE PROPTB_TUSS_ID = 5127
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5129 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.373' WHERE PROPTB_TUSS_ID = 5128
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5130 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.490' WHERE PROPTB_TUSS_ID = 5129
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5131 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.613' WHERE PROPTB_TUSS_ID = 5130
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5132 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.743' WHERE PROPTB_TUSS_ID = 5131
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5133 , CLOUD_SYNC_DATE = '2014-05-06 17:40:28.887' WHERE PROPTB_TUSS_ID = 5132
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5134 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.010' WHERE PROPTB_TUSS_ID = 5133
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5135 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.133' WHERE PROPTB_TUSS_ID = 5134
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5136 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.257' WHERE PROPTB_TUSS_ID = 5135
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5137 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.380' WHERE PROPTB_TUSS_ID = 5136
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5138 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.497' WHERE PROPTB_TUSS_ID = 5137
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5139 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.617' WHERE PROPTB_TUSS_ID = 5138
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5140 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.740' WHERE PROPTB_TUSS_ID = 5139
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5141 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.860' WHERE PROPTB_TUSS_ID = 5140
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5142 , CLOUD_SYNC_DATE = '2014-05-06 17:40:29.980' WHERE PROPTB_TUSS_ID = 5141
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5143 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.100' WHERE PROPTB_TUSS_ID = 5142
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5144 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.220' WHERE PROPTB_TUSS_ID = 5143
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5145 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.337' WHERE PROPTB_TUSS_ID = 5144
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5146 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.457' WHERE PROPTB_TUSS_ID = 5145
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5147 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.573' WHERE PROPTB_TUSS_ID = 5146
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5148 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.697' WHERE PROPTB_TUSS_ID = 5147
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5149 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.813' WHERE PROPTB_TUSS_ID = 5148
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5150 , CLOUD_SYNC_DATE = '2014-05-06 17:40:30.910' WHERE PROPTB_TUSS_ID = 5149
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5151 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.053' WHERE PROPTB_TUSS_ID = 5150
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5152 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.173' WHERE PROPTB_TUSS_ID = 5151
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5153 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.293' WHERE PROPTB_TUSS_ID = 5152
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5154 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.410' WHERE PROPTB_TUSS_ID = 5153
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5155 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.527' WHERE PROPTB_TUSS_ID = 5154
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5156 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.643' WHERE PROPTB_TUSS_ID = 5155
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5157 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.767' WHERE PROPTB_TUSS_ID = 5156
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5158 , CLOUD_SYNC_DATE = '2014-05-06 17:40:31.887' WHERE PROPTB_TUSS_ID = 5157
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5159 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.003' WHERE PROPTB_TUSS_ID = 5158
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5160 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.127' WHERE PROPTB_TUSS_ID = 5159
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5161 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.243' WHERE PROPTB_TUSS_ID = 5160
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5162 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.363' WHERE PROPTB_TUSS_ID = 5161
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5163 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.480' WHERE PROPTB_TUSS_ID = 5162
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5164 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.603' WHERE PROPTB_TUSS_ID = 5163
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5165 , CLOUD_SYNC_DATE = '2014-05-06 17:40:32.723' WHERE PROPTB_TUSS_ID = 5164
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4571 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.463' WHERE PROPTB_TUSS_ID = 5165
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4572 , CLOUD_SYNC_DATE = '2014-05-06 17:39:25.567' WHERE PROPTB_TUSS_ID = 5166
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5168 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.087' WHERE PROPTB_TUSS_ID = 5167
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5169 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.207' WHERE PROPTB_TUSS_ID = 5168
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5170 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.327' WHERE PROPTB_TUSS_ID = 5169
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5171 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.447' WHERE PROPTB_TUSS_ID = 5170
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5172 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.563' WHERE PROPTB_TUSS_ID = 5171
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5173 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.693' WHERE PROPTB_TUSS_ID = 5172
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5174 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.817' WHERE PROPTB_TUSS_ID = 5173
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5175 , CLOUD_SYNC_DATE = '2014-05-06 17:40:33.937' WHERE PROPTB_TUSS_ID = 5174
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5176 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.067' WHERE PROPTB_TUSS_ID = 5175
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5177 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.180' WHERE PROPTB_TUSS_ID = 5176
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5178 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.303' WHERE PROPTB_TUSS_ID = 5177
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5179 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.427' WHERE PROPTB_TUSS_ID = 5178
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5180 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.540' WHERE PROPTB_TUSS_ID = 5179
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5181 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.663' WHERE PROPTB_TUSS_ID = 5180
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5182 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.780' WHERE PROPTB_TUSS_ID = 5181
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5183 , CLOUD_SYNC_DATE = '2014-05-06 17:40:34.900' WHERE PROPTB_TUSS_ID = 5182
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5184 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.020' WHERE PROPTB_TUSS_ID = 5183
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5185 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.137' WHERE PROPTB_TUSS_ID = 5184
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5185 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.137' WHERE PROPTB_TUSS_ID = 5185
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5187 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.373' WHERE PROPTB_TUSS_ID = 5186
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5188 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.493' WHERE PROPTB_TUSS_ID = 5187
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5189 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.617' WHERE PROPTB_TUSS_ID = 5188
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5190 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.773' WHERE PROPTB_TUSS_ID = 5189
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5191 , CLOUD_SYNC_DATE = '2014-05-06 17:40:35.940' WHERE PROPTB_TUSS_ID = 5190
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5192 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.110' WHERE PROPTB_TUSS_ID = 5191
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5193 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.240' WHERE PROPTB_TUSS_ID = 5192
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5194 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.363' WHERE PROPTB_TUSS_ID = 5193
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5195 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.480' WHERE PROPTB_TUSS_ID = 5194
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5196 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.603' WHERE PROPTB_TUSS_ID = 5195
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5197 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.727' WHERE PROPTB_TUSS_ID = 5196
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5198 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.847' WHERE PROPTB_TUSS_ID = 5197
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5199 , CLOUD_SYNC_DATE = '2014-05-06 17:40:36.970' WHERE PROPTB_TUSS_ID = 5198
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5200 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.087' WHERE PROPTB_TUSS_ID = 5199
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5201 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.213' WHERE PROPTB_TUSS_ID = 5200
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5202 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.330' WHERE PROPTB_TUSS_ID = 5201
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5203 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.447' WHERE PROPTB_TUSS_ID = 5202
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5204 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.570' WHERE PROPTB_TUSS_ID = 5203
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5205 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.687' WHERE PROPTB_TUSS_ID = 5204
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5206 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.810' WHERE PROPTB_TUSS_ID = 5205
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5207 , CLOUD_SYNC_DATE = '2014-05-06 17:40:37.930' WHERE PROPTB_TUSS_ID = 5206
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5208 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.047' WHERE PROPTB_TUSS_ID = 5207
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5209 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.167' WHERE PROPTB_TUSS_ID = 5208
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5210 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.283' WHERE PROPTB_TUSS_ID = 5209
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5211 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.403' WHERE PROPTB_TUSS_ID = 5210
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5212 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.523' WHERE PROPTB_TUSS_ID = 5211
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5213 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.643' WHERE PROPTB_TUSS_ID = 5212
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5214 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.767' WHERE PROPTB_TUSS_ID = 5213
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5215 , CLOUD_SYNC_DATE = '2014-05-06 17:40:38.893' WHERE PROPTB_TUSS_ID = 5214
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5216 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.010' WHERE PROPTB_TUSS_ID = 5215
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5217 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.127' WHERE PROPTB_TUSS_ID = 5216
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5218 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.247' WHERE PROPTB_TUSS_ID = 5217
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5219 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.367' WHERE PROPTB_TUSS_ID = 5218
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5220 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.490' WHERE PROPTB_TUSS_ID = 5219
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5221 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.607' WHERE PROPTB_TUSS_ID = 5220
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5222 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.727' WHERE PROPTB_TUSS_ID = 5221
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5223 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.850' WHERE PROPTB_TUSS_ID = 5222
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5224 , CLOUD_SYNC_DATE = '2014-05-06 17:40:39.967' WHERE PROPTB_TUSS_ID = 5223
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5225 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.087' WHERE PROPTB_TUSS_ID = 5224
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5226 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.203' WHERE PROPTB_TUSS_ID = 5225
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5227 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.327' WHERE PROPTB_TUSS_ID = 5226
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5228 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.450' WHERE PROPTB_TUSS_ID = 5227
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5229 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.567' WHERE PROPTB_TUSS_ID = 5228
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5230 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.693' WHERE PROPTB_TUSS_ID = 5229
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5231 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.813' WHERE PROPTB_TUSS_ID = 5230
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5232 , CLOUD_SYNC_DATE = '2014-05-06 17:40:40.933' WHERE PROPTB_TUSS_ID = 5231
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5233 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.057' WHERE PROPTB_TUSS_ID = 5232
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5234 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.177' WHERE PROPTB_TUSS_ID = 5233
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5235 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.297' WHERE PROPTB_TUSS_ID = 5234
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5236 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.420' WHERE PROPTB_TUSS_ID = 5235
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5237 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.543' WHERE PROPTB_TUSS_ID = 5236
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5238 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.663' WHERE PROPTB_TUSS_ID = 5237
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5239 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.783' WHERE PROPTB_TUSS_ID = 5238
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5240 , CLOUD_SYNC_DATE = '2014-05-06 17:40:41.917' WHERE PROPTB_TUSS_ID = 5239
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5241 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.037' WHERE PROPTB_TUSS_ID = 5240
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5242 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.160' WHERE PROPTB_TUSS_ID = 5241
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5243 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.280' WHERE PROPTB_TUSS_ID = 5242
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5244 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.400' WHERE PROPTB_TUSS_ID = 5243
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5245 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.523' WHERE PROPTB_TUSS_ID = 5244
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5246 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.643' WHERE PROPTB_TUSS_ID = 5245
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5247 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.773' WHERE PROPTB_TUSS_ID = 5246
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5248 , CLOUD_SYNC_DATE = '2014-05-06 17:40:42.897' WHERE PROPTB_TUSS_ID = 5247
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5249 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.013' WHERE PROPTB_TUSS_ID = 5248
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5250 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.137' WHERE PROPTB_TUSS_ID = 5249
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5251 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.260' WHERE PROPTB_TUSS_ID = 5250
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5252 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.377' WHERE PROPTB_TUSS_ID = 5251
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5253 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.500' WHERE PROPTB_TUSS_ID = 5252
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5254 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.623' WHERE PROPTB_TUSS_ID = 5253
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5255 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.743' WHERE PROPTB_TUSS_ID = 5254
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5256 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.867' WHERE PROPTB_TUSS_ID = 5255
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5257 , CLOUD_SYNC_DATE = '2014-05-06 17:40:43.987' WHERE PROPTB_TUSS_ID = 5256
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5258 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.113' WHERE PROPTB_TUSS_ID = 5257
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5259 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.237' WHERE PROPTB_TUSS_ID = 5258
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5260 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.360' WHERE PROPTB_TUSS_ID = 5259
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5261 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.477' WHERE PROPTB_TUSS_ID = 5260
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5262 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.600' WHERE PROPTB_TUSS_ID = 5261
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5263 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.727' WHERE PROPTB_TUSS_ID = 5262
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5264 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.850' WHERE PROPTB_TUSS_ID = 5263
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5265 , CLOUD_SYNC_DATE = '2014-05-06 17:40:44.967' WHERE PROPTB_TUSS_ID = 5264
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5266 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.090' WHERE PROPTB_TUSS_ID = 5265
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5267 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.217' WHERE PROPTB_TUSS_ID = 5266
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5268 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.337' WHERE PROPTB_TUSS_ID = 5267
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5269 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.457' WHERE PROPTB_TUSS_ID = 5268
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5270 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.580' WHERE PROPTB_TUSS_ID = 5269
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5271 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.697' WHERE PROPTB_TUSS_ID = 5270
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5272 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.820' WHERE PROPTB_TUSS_ID = 5271
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5273 , CLOUD_SYNC_DATE = '2014-05-06 17:40:45.940' WHERE PROPTB_TUSS_ID = 5272
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5274 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.060' WHERE PROPTB_TUSS_ID = 5273
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5275 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.183' WHERE PROPTB_TUSS_ID = 5274
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5276 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.307' WHERE PROPTB_TUSS_ID = 5275
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5277 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.423' WHERE PROPTB_TUSS_ID = 5276
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5278 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.550' WHERE PROPTB_TUSS_ID = 5277
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5279 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.680' WHERE PROPTB_TUSS_ID = 5278
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5280 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.803' WHERE PROPTB_TUSS_ID = 5279
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5281 , CLOUD_SYNC_DATE = '2014-05-06 17:40:46.923' WHERE PROPTB_TUSS_ID = 5280
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5282 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.043' WHERE PROPTB_TUSS_ID = 5281
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5283 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.167' WHERE PROPTB_TUSS_ID = 5282
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5284 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.283' WHERE PROPTB_TUSS_ID = 5283
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5285 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.407' WHERE PROPTB_TUSS_ID = 5284
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5286 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.527' WHERE PROPTB_TUSS_ID = 5285
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5287 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.650' WHERE PROPTB_TUSS_ID = 5286
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5288 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.773' WHERE PROPTB_TUSS_ID = 5287
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5289 , CLOUD_SYNC_DATE = '2014-05-06 17:40:47.900' WHERE PROPTB_TUSS_ID = 5288
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5290 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.023' WHERE PROPTB_TUSS_ID = 5289
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5291 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.143' WHERE PROPTB_TUSS_ID = 5290
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5292 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.267' WHERE PROPTB_TUSS_ID = 5291
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5293 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.393' WHERE PROPTB_TUSS_ID = 5292
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5294 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.513' WHERE PROPTB_TUSS_ID = 5293
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5295 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.640' WHERE PROPTB_TUSS_ID = 5294
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5296 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.763' WHERE PROPTB_TUSS_ID = 5295
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5297 , CLOUD_SYNC_DATE = '2014-05-06 17:40:48.890' WHERE PROPTB_TUSS_ID = 5296
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5298 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.007' WHERE PROPTB_TUSS_ID = 5297
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5299 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.133' WHERE PROPTB_TUSS_ID = 5298
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5300 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.257' WHERE PROPTB_TUSS_ID = 5299
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5301 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.373' WHERE PROPTB_TUSS_ID = 5300
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5302 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.497' WHERE PROPTB_TUSS_ID = 5301
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5303 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.620' WHERE PROPTB_TUSS_ID = 5302
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5304 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.747' WHERE PROPTB_TUSS_ID = 5303
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5305 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.867' WHERE PROPTB_TUSS_ID = 5304
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5306 , CLOUD_SYNC_DATE = '2014-05-06 17:40:49.993' WHERE PROPTB_TUSS_ID = 5305
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5307 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.117' WHERE PROPTB_TUSS_ID = 5306
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5308 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.237' WHERE PROPTB_TUSS_ID = 5307
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5309 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.357' WHERE PROPTB_TUSS_ID = 5308
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5310 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.480' WHERE PROPTB_TUSS_ID = 5309
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5311 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.607' WHERE PROPTB_TUSS_ID = 5310
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5312 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.730' WHERE PROPTB_TUSS_ID = 5311
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5313 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.853' WHERE PROPTB_TUSS_ID = 5312
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5314 , CLOUD_SYNC_DATE = '2014-05-06 17:40:50.977' WHERE PROPTB_TUSS_ID = 5313
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5315 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.100' WHERE PROPTB_TUSS_ID = 5314
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5316 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.217' WHERE PROPTB_TUSS_ID = 5315
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5317 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.347' WHERE PROPTB_TUSS_ID = 5316
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5318 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.470' WHERE PROPTB_TUSS_ID = 5317
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5319 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.587' WHERE PROPTB_TUSS_ID = 5318
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5320 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.713' WHERE PROPTB_TUSS_ID = 5319
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5321 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.837' WHERE PROPTB_TUSS_ID = 5320
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5322 , CLOUD_SYNC_DATE = '2014-05-06 17:40:51.957' WHERE PROPTB_TUSS_ID = 5321
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5323 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.077' WHERE PROPTB_TUSS_ID = 5322
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5324 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.197' WHERE PROPTB_TUSS_ID = 5323
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5325 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.320' WHERE PROPTB_TUSS_ID = 5324
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5326 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.443' WHERE PROPTB_TUSS_ID = 5325
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5327 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.563' WHERE PROPTB_TUSS_ID = 5326
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5328 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.687' WHERE PROPTB_TUSS_ID = 5327
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5329 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.810' WHERE PROPTB_TUSS_ID = 5328
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5330 , CLOUD_SYNC_DATE = '2014-05-06 17:40:52.933' WHERE PROPTB_TUSS_ID = 5329
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5331 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.053' WHERE PROPTB_TUSS_ID = 5330
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5332 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.177' WHERE PROPTB_TUSS_ID = 5331
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5333 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.303' WHERE PROPTB_TUSS_ID = 5332
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5334 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.430' WHERE PROPTB_TUSS_ID = 5333
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5335 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.550' WHERE PROPTB_TUSS_ID = 5334
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5336 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.687' WHERE PROPTB_TUSS_ID = 5335
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5337 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.813' WHERE PROPTB_TUSS_ID = 5336
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5338 , CLOUD_SYNC_DATE = '2014-05-06 17:40:53.937' WHERE PROPTB_TUSS_ID = 5337
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5339 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.057' WHERE PROPTB_TUSS_ID = 5338
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5340 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.193' WHERE PROPTB_TUSS_ID = 5339
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5341 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.317' WHERE PROPTB_TUSS_ID = 5340
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5342 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.440' WHERE PROPTB_TUSS_ID = 5341
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5343 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.557' WHERE PROPTB_TUSS_ID = 5342
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5344 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.687' WHERE PROPTB_TUSS_ID = 5343
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5345 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.813' WHERE PROPTB_TUSS_ID = 5344
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5346 , CLOUD_SYNC_DATE = '2014-05-06 17:40:54.937' WHERE PROPTB_TUSS_ID = 5345
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5347 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.060' WHERE PROPTB_TUSS_ID = 5346
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5348 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.183' WHERE PROPTB_TUSS_ID = 5347
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5349 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.307' WHERE PROPTB_TUSS_ID = 5348
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5350 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.433' WHERE PROPTB_TUSS_ID = 5349
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5351 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.557' WHERE PROPTB_TUSS_ID = 5350
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5352 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.683' WHERE PROPTB_TUSS_ID = 5351
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5353 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.807' WHERE PROPTB_TUSS_ID = 5352
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5354 , CLOUD_SYNC_DATE = '2014-05-06 17:40:55.930' WHERE PROPTB_TUSS_ID = 5353
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5355 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.053' WHERE PROPTB_TUSS_ID = 5354
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5356 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.173' WHERE PROPTB_TUSS_ID = 5355
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5357 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.297' WHERE PROPTB_TUSS_ID = 5356
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5358 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.423' WHERE PROPTB_TUSS_ID = 5357
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5359 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.547' WHERE PROPTB_TUSS_ID = 5358
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5360 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.670' WHERE PROPTB_TUSS_ID = 5359
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5361 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.790' WHERE PROPTB_TUSS_ID = 5360
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5362 , CLOUD_SYNC_DATE = '2014-05-06 17:40:56.917' WHERE PROPTB_TUSS_ID = 5361
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5363 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.040' WHERE PROPTB_TUSS_ID = 5362
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5364 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.167' WHERE PROPTB_TUSS_ID = 5363
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5365 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.290' WHERE PROPTB_TUSS_ID = 5364
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5366 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.410' WHERE PROPTB_TUSS_ID = 5365
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5367 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.533' WHERE PROPTB_TUSS_ID = 5366
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5368 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.660' WHERE PROPTB_TUSS_ID = 5367
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5369 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.783' WHERE PROPTB_TUSS_ID = 5368
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5370 , CLOUD_SYNC_DATE = '2014-05-06 17:40:57.910' WHERE PROPTB_TUSS_ID = 5369
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5371 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.027' WHERE PROPTB_TUSS_ID = 5370
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5372 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.150' WHERE PROPTB_TUSS_ID = 5371
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5373 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.277' WHERE PROPTB_TUSS_ID = 5372
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5374 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.400' WHERE PROPTB_TUSS_ID = 5373
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5375 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.520' WHERE PROPTB_TUSS_ID = 5374
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5376 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.643' WHERE PROPTB_TUSS_ID = 5375
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5377 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.770' WHERE PROPTB_TUSS_ID = 5376
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5378 , CLOUD_SYNC_DATE = '2014-05-06 17:40:58.897' WHERE PROPTB_TUSS_ID = 5377
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5379 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.023' WHERE PROPTB_TUSS_ID = 5378
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5380 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.143' WHERE PROPTB_TUSS_ID = 5379
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5381 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.267' WHERE PROPTB_TUSS_ID = 5380
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5382 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.393' WHERE PROPTB_TUSS_ID = 5381
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5383 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.517' WHERE PROPTB_TUSS_ID = 5382
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5384 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.643' WHERE PROPTB_TUSS_ID = 5383
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5385 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.767' WHERE PROPTB_TUSS_ID = 5384
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5386 , CLOUD_SYNC_DATE = '2014-05-06 17:40:59.890' WHERE PROPTB_TUSS_ID = 5385
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5387 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.013' WHERE PROPTB_TUSS_ID = 5386
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5388 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.137' WHERE PROPTB_TUSS_ID = 5387
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5389 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.257' WHERE PROPTB_TUSS_ID = 5388
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5390 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.380' WHERE PROPTB_TUSS_ID = 5389
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5391 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.497' WHERE PROPTB_TUSS_ID = 5390
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5392 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.627' WHERE PROPTB_TUSS_ID = 5391
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5393 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.753' WHERE PROPTB_TUSS_ID = 5392
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5394 , CLOUD_SYNC_DATE = '2014-05-06 17:41:00.877' WHERE PROPTB_TUSS_ID = 5393
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5395 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.000' WHERE PROPTB_TUSS_ID = 5394
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5396 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.123' WHERE PROPTB_TUSS_ID = 5395
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5397 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.247' WHERE PROPTB_TUSS_ID = 5396
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5398 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.373' WHERE PROPTB_TUSS_ID = 5397
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5399 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.500' WHERE PROPTB_TUSS_ID = 5398
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5400 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.627' WHERE PROPTB_TUSS_ID = 5399
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5401 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.753' WHERE PROPTB_TUSS_ID = 5400
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5402 , CLOUD_SYNC_DATE = '2014-05-06 17:41:01.873' WHERE PROPTB_TUSS_ID = 5401
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5403 , CLOUD_SYNC_DATE = '2014-05-06 17:41:02.000' WHERE PROPTB_TUSS_ID = 5402
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5404 , CLOUD_SYNC_DATE = '2014-05-06 17:41:02.123' WHERE PROPTB_TUSS_ID = 5403
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5405 , CLOUD_SYNC_DATE = '2014-05-06 17:41:02.250' WHERE PROPTB_TUSS_ID = 5404
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5406 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.553' WHERE PROPTB_TUSS_ID = 5405
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5407 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.643' WHERE PROPTB_TUSS_ID = 5406
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5408 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.483' WHERE PROPTB_TUSS_ID = 5407
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5409 , CLOUD_SYNC_DATE = '2014-05-06 17:38:45.767' WHERE PROPTB_TUSS_ID = 5408
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5410 , CLOUD_SYNC_DATE = '2014-05-06 17:35:30.540' WHERE PROPTB_TUSS_ID = 5409
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5411 , CLOUD_SYNC_DATE = '2014-05-06 17:41:02.993' WHERE PROPTB_TUSS_ID = 5410
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5412 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.117' WHERE PROPTB_TUSS_ID = 5411
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5413 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.243' WHERE PROPTB_TUSS_ID = 5412
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5414 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.363' WHERE PROPTB_TUSS_ID = 5413
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5415 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.490' WHERE PROPTB_TUSS_ID = 5414
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5416 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.617' WHERE PROPTB_TUSS_ID = 5415
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5417 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.747' WHERE PROPTB_TUSS_ID = 5416
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5418 , CLOUD_SYNC_DATE = '2014-05-06 17:41:03.877' WHERE PROPTB_TUSS_ID = 5417
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5419 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.007' WHERE PROPTB_TUSS_ID = 5418
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5420 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.127' WHERE PROPTB_TUSS_ID = 5419
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5421 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.290' WHERE PROPTB_TUSS_ID = 5420
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5422 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.417' WHERE PROPTB_TUSS_ID = 5421
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5423 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.540' WHERE PROPTB_TUSS_ID = 5422
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5424 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.667' WHERE PROPTB_TUSS_ID = 5423
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5425 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.793' WHERE PROPTB_TUSS_ID = 5424
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5426 , CLOUD_SYNC_DATE = '2014-05-06 17:41:04.920' WHERE PROPTB_TUSS_ID = 5425
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5427 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.047' WHERE PROPTB_TUSS_ID = 5426
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5428 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.173' WHERE PROPTB_TUSS_ID = 5427
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5429 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.300' WHERE PROPTB_TUSS_ID = 5428
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5430 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.423' WHERE PROPTB_TUSS_ID = 5429
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5431 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.550' WHERE PROPTB_TUSS_ID = 5430
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5432 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.680' WHERE PROPTB_TUSS_ID = 5431
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5433 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.803' WHERE PROPTB_TUSS_ID = 5432
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5434 , CLOUD_SYNC_DATE = '2014-05-06 17:41:05.927' WHERE PROPTB_TUSS_ID = 5433
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5435 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.053' WHERE PROPTB_TUSS_ID = 5434
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5436 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.180' WHERE PROPTB_TUSS_ID = 5435
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5437 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.327' WHERE PROPTB_TUSS_ID = 5436
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5438 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.453' WHERE PROPTB_TUSS_ID = 5437
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5439 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.577' WHERE PROPTB_TUSS_ID = 5438
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5440 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.707' WHERE PROPTB_TUSS_ID = 5439
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5441 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.830' WHERE PROPTB_TUSS_ID = 5440
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5442 , CLOUD_SYNC_DATE = '2014-05-06 17:41:06.953' WHERE PROPTB_TUSS_ID = 5441
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5443 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.077' WHERE PROPTB_TUSS_ID = 5442
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5444 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.203' WHERE PROPTB_TUSS_ID = 5443
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5445 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.330' WHERE PROPTB_TUSS_ID = 5444
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5446 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.453' WHERE PROPTB_TUSS_ID = 5445
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5447 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.580' WHERE PROPTB_TUSS_ID = 5446
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5448 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.707' WHERE PROPTB_TUSS_ID = 5447
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5449 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.830' WHERE PROPTB_TUSS_ID = 5448
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5450 , CLOUD_SYNC_DATE = '2014-05-06 17:41:07.950' WHERE PROPTB_TUSS_ID = 5449
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5451 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.077' WHERE PROPTB_TUSS_ID = 5450
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5452 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.203' WHERE PROPTB_TUSS_ID = 5451
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5453 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.330' WHERE PROPTB_TUSS_ID = 5452
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5454 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.453' WHERE PROPTB_TUSS_ID = 5453
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5455 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.577' WHERE PROPTB_TUSS_ID = 5454
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5456 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.707' WHERE PROPTB_TUSS_ID = 5455
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5457 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.837' WHERE PROPTB_TUSS_ID = 5456
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5458 , CLOUD_SYNC_DATE = '2014-05-06 17:41:08.960' WHERE PROPTB_TUSS_ID = 5457
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5459 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.087' WHERE PROPTB_TUSS_ID = 5458
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5460 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.210' WHERE PROPTB_TUSS_ID = 5459
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5461 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.337' WHERE PROPTB_TUSS_ID = 5460
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5462 , CLOUD_SYNC_DATE = '2014-05-06 17:35:29.523' WHERE PROPTB_TUSS_ID = 5461
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5463 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.590' WHERE PROPTB_TUSS_ID = 5462
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5464 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.723' WHERE PROPTB_TUSS_ID = 5463
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5465 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.847' WHERE PROPTB_TUSS_ID = 5464
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5466 , CLOUD_SYNC_DATE = '2014-05-06 17:41:09.977' WHERE PROPTB_TUSS_ID = 5465
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5467 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.103' WHERE PROPTB_TUSS_ID = 5466
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5468 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.227' WHERE PROPTB_TUSS_ID = 5467
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5469 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.350' WHERE PROPTB_TUSS_ID = 5468
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5470 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.477' WHERE PROPTB_TUSS_ID = 5469
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5471 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.603' WHERE PROPTB_TUSS_ID = 5470
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5472 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.733' WHERE PROPTB_TUSS_ID = 5471
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5473 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.860' WHERE PROPTB_TUSS_ID = 5472
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5474 , CLOUD_SYNC_DATE = '2014-05-06 17:41:10.987' WHERE PROPTB_TUSS_ID = 5473
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5475 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.117' WHERE PROPTB_TUSS_ID = 5474
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5476 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.240' WHERE PROPTB_TUSS_ID = 5475
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5477 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.367' WHERE PROPTB_TUSS_ID = 5476
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5478 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.487' WHERE PROPTB_TUSS_ID = 5477
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5479 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.617' WHERE PROPTB_TUSS_ID = 5478
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5480 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.747' WHERE PROPTB_TUSS_ID = 5479
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5481 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.873' WHERE PROPTB_TUSS_ID = 5480
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5482 , CLOUD_SYNC_DATE = '2014-05-06 17:41:11.997' WHERE PROPTB_TUSS_ID = 5481
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5483 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.123' WHERE PROPTB_TUSS_ID = 5482
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5484 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.247' WHERE PROPTB_TUSS_ID = 5483
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5485 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.373' WHERE PROPTB_TUSS_ID = 5484
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5486 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.500' WHERE PROPTB_TUSS_ID = 5485
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5487 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.627' WHERE PROPTB_TUSS_ID = 5486
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5488 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.757' WHERE PROPTB_TUSS_ID = 5487
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5489 , CLOUD_SYNC_DATE = '2014-05-06 17:41:12.887' WHERE PROPTB_TUSS_ID = 5488
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5490 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.013' WHERE PROPTB_TUSS_ID = 5489
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5491 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.140' WHERE PROPTB_TUSS_ID = 5490
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5492 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.270' WHERE PROPTB_TUSS_ID = 5491
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5493 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.410' WHERE PROPTB_TUSS_ID = 5492
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5494 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.557' WHERE PROPTB_TUSS_ID = 5493
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5495 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.690' WHERE PROPTB_TUSS_ID = 5494
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5496 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.817' WHERE PROPTB_TUSS_ID = 5495
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5497 , CLOUD_SYNC_DATE = '2014-05-06 17:41:13.970' WHERE PROPTB_TUSS_ID = 5496
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5498 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.097' WHERE PROPTB_TUSS_ID = 5497
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5499 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.220' WHERE PROPTB_TUSS_ID = 5498
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5500 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.357' WHERE PROPTB_TUSS_ID = 5499
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5501 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.483' WHERE PROPTB_TUSS_ID = 5500
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5502 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.610' WHERE PROPTB_TUSS_ID = 5501
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5503 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.743' WHERE PROPTB_TUSS_ID = 5502
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5504 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.863' WHERE PROPTB_TUSS_ID = 5503
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5505 , CLOUD_SYNC_DATE = '2014-05-06 17:41:14.993' WHERE PROPTB_TUSS_ID = 5504
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5506 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.117' WHERE PROPTB_TUSS_ID = 5505
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5507 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.243' WHERE PROPTB_TUSS_ID = 5506
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5508 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.370' WHERE PROPTB_TUSS_ID = 5507
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5509 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.497' WHERE PROPTB_TUSS_ID = 5508
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5510 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.623' WHERE PROPTB_TUSS_ID = 5509
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5511 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.757' WHERE PROPTB_TUSS_ID = 5510
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5512 , CLOUD_SYNC_DATE = '2014-05-06 17:41:15.880' WHERE PROPTB_TUSS_ID = 5511
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5513 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.010' WHERE PROPTB_TUSS_ID = 5512
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5514 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.137' WHERE PROPTB_TUSS_ID = 5513
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5515 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.267' WHERE PROPTB_TUSS_ID = 5514
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5516 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.393' WHERE PROPTB_TUSS_ID = 5515
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5517 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.520' WHERE PROPTB_TUSS_ID = 5516
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5518 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.650' WHERE PROPTB_TUSS_ID = 5517
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5519 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.783' WHERE PROPTB_TUSS_ID = 5518
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5520 , CLOUD_SYNC_DATE = '2014-05-06 17:41:16.913' WHERE PROPTB_TUSS_ID = 5519
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5521 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.043' WHERE PROPTB_TUSS_ID = 5520
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 4773 , CLOUD_SYNC_DATE = '2014-05-06 17:39:47.433' WHERE PROPTB_TUSS_ID = 5521
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5523 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.300' WHERE PROPTB_TUSS_ID = 5522
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5524 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.427' WHERE PROPTB_TUSS_ID = 5523
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5525 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.553' WHERE PROPTB_TUSS_ID = 5524
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5526 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.683' WHERE PROPTB_TUSS_ID = 5525
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5527 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.810' WHERE PROPTB_TUSS_ID = 5526
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5528 , CLOUD_SYNC_DATE = '2014-05-06 17:41:17.937' WHERE PROPTB_TUSS_ID = 5527
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5529 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.063' WHERE PROPTB_TUSS_ID = 5528
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5530 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.193' WHERE PROPTB_TUSS_ID = 5529
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5531 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.320' WHERE PROPTB_TUSS_ID = 5530
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5532 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.450' WHERE PROPTB_TUSS_ID = 5531
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5533 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.577' WHERE PROPTB_TUSS_ID = 5532
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5534 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.707' WHERE PROPTB_TUSS_ID = 5533
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5535 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.837' WHERE PROPTB_TUSS_ID = 5534
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5536 , CLOUD_SYNC_DATE = '2014-05-06 17:41:18.963' WHERE PROPTB_TUSS_ID = 5535
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5537 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.093' WHERE PROPTB_TUSS_ID = 5536
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5538 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.240' WHERE PROPTB_TUSS_ID = 5537
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5539 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.383' WHERE PROPTB_TUSS_ID = 5538
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5540 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.520' WHERE PROPTB_TUSS_ID = 5539
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5541 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.670' WHERE PROPTB_TUSS_ID = 5540
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5542 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.817' WHERE PROPTB_TUSS_ID = 5541
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5543 , CLOUD_SYNC_DATE = '2014-05-06 17:41:19.980' WHERE PROPTB_TUSS_ID = 5542
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5544 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.113' WHERE PROPTB_TUSS_ID = 5543
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5545 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.243' WHERE PROPTB_TUSS_ID = 5544
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5546 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.433' WHERE PROPTB_TUSS_ID = 5545
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5547 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.560' WHERE PROPTB_TUSS_ID = 5546
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5548 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.693' WHERE PROPTB_TUSS_ID = 5547
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5549 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.823' WHERE PROPTB_TUSS_ID = 5548
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5550 , CLOUD_SYNC_DATE = '2014-05-06 17:41:20.950' WHERE PROPTB_TUSS_ID = 5549
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5551 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.080' WHERE PROPTB_TUSS_ID = 5550
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5552 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.203' WHERE PROPTB_TUSS_ID = 5551
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5553 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.333' WHERE PROPTB_TUSS_ID = 5552
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5554 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.460' WHERE PROPTB_TUSS_ID = 5553
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5555 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.590' WHERE PROPTB_TUSS_ID = 5554
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5556 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.723' WHERE PROPTB_TUSS_ID = 5555
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5557 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.850' WHERE PROPTB_TUSS_ID = 5556
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5558 , CLOUD_SYNC_DATE = '2014-05-06 17:41:21.980' WHERE PROPTB_TUSS_ID = 5557
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5559 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.110' WHERE PROPTB_TUSS_ID = 5558
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5560 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.237' WHERE PROPTB_TUSS_ID = 5559
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5561 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.363' WHERE PROPTB_TUSS_ID = 5560
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5562 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.493' WHERE PROPTB_TUSS_ID = 5561
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5563 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.620' WHERE PROPTB_TUSS_ID = 5562
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5564 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.753' WHERE PROPTB_TUSS_ID = 5563
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5565 , CLOUD_SYNC_DATE = '2014-05-06 17:41:22.883' WHERE PROPTB_TUSS_ID = 5564
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5566 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.010' WHERE PROPTB_TUSS_ID = 5565
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5567 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.140' WHERE PROPTB_TUSS_ID = 5566
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5568 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.267' WHERE PROPTB_TUSS_ID = 5567
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5569 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.397' WHERE PROPTB_TUSS_ID = 5568
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5570 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.523' WHERE PROPTB_TUSS_ID = 5569
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5571 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.657' WHERE PROPTB_TUSS_ID = 5570
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5572 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.790' WHERE PROPTB_TUSS_ID = 5571
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5573 , CLOUD_SYNC_DATE = '2014-05-06 17:41:23.917' WHERE PROPTB_TUSS_ID = 5572
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5574 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.047' WHERE PROPTB_TUSS_ID = 5573
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5575 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.177' WHERE PROPTB_TUSS_ID = 5574
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5576 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.307' WHERE PROPTB_TUSS_ID = 5575
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5577 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.450' WHERE PROPTB_TUSS_ID = 5576
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5578 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.583' WHERE PROPTB_TUSS_ID = 5577
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5579 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.717' WHERE PROPTB_TUSS_ID = 5578
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5580 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.847' WHERE PROPTB_TUSS_ID = 5579
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5581 , CLOUD_SYNC_DATE = '2014-05-06 17:41:24.977' WHERE PROPTB_TUSS_ID = 5580
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5582 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.110' WHERE PROPTB_TUSS_ID = 5581
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5583 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.240' WHERE PROPTB_TUSS_ID = 5582
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5584 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.370' WHERE PROPTB_TUSS_ID = 5583
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5585 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.500' WHERE PROPTB_TUSS_ID = 5584
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5586 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.630' WHERE PROPTB_TUSS_ID = 5585
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5587 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.763' WHERE PROPTB_TUSS_ID = 5586
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5588 , CLOUD_SYNC_DATE = '2014-05-06 17:41:25.893' WHERE PROPTB_TUSS_ID = 5587
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5589 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.023' WHERE PROPTB_TUSS_ID = 5588
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5590 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.150' WHERE PROPTB_TUSS_ID = 5589
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5591 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.283' WHERE PROPTB_TUSS_ID = 5590
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5592 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.410' WHERE PROPTB_TUSS_ID = 5591
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5593 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.540' WHERE PROPTB_TUSS_ID = 5592
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5594 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.670' WHERE PROPTB_TUSS_ID = 5593
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5595 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.800' WHERE PROPTB_TUSS_ID = 5594
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5596 , CLOUD_SYNC_DATE = '2014-05-06 17:41:26.930' WHERE PROPTB_TUSS_ID = 5595
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5597 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.060' WHERE PROPTB_TUSS_ID = 5596
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5598 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.190' WHERE PROPTB_TUSS_ID = 5597
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5599 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.320' WHERE PROPTB_TUSS_ID = 5598
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5600 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.450' WHERE PROPTB_TUSS_ID = 5599
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5601 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.580' WHERE PROPTB_TUSS_ID = 5600
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5602 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.720' WHERE PROPTB_TUSS_ID = 5601
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5603 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.847' WHERE PROPTB_TUSS_ID = 5602
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5604 , CLOUD_SYNC_DATE = '2014-05-06 17:41:27.977' WHERE PROPTB_TUSS_ID = 5603
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5605 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.103' WHERE PROPTB_TUSS_ID = 5604
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5606 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.233' WHERE PROPTB_TUSS_ID = 5605
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5607 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.360' WHERE PROPTB_TUSS_ID = 5606
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5608 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.497' WHERE PROPTB_TUSS_ID = 5607
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5609 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.623' WHERE PROPTB_TUSS_ID = 5608
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5610 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.760' WHERE PROPTB_TUSS_ID = 5609
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5611 , CLOUD_SYNC_DATE = '2014-05-06 17:41:28.890' WHERE PROPTB_TUSS_ID = 5610
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5612 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.020' WHERE PROPTB_TUSS_ID = 5611
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5613 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.150' WHERE PROPTB_TUSS_ID = 5612
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5614 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.280' WHERE PROPTB_TUSS_ID = 5613
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5615 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.410' WHERE PROPTB_TUSS_ID = 5614
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5616 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.543' WHERE PROPTB_TUSS_ID = 5615
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5617 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.673' WHERE PROPTB_TUSS_ID = 5616
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5618 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.807' WHERE PROPTB_TUSS_ID = 5617
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5619 , CLOUD_SYNC_DATE = '2014-05-06 17:41:29.937' WHERE PROPTB_TUSS_ID = 5618
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5620 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.067' WHERE PROPTB_TUSS_ID = 5619
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5621 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.197' WHERE PROPTB_TUSS_ID = 5620
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5622 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.327' WHERE PROPTB_TUSS_ID = 5621
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5623 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.460' WHERE PROPTB_TUSS_ID = 5622
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5624 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.587' WHERE PROPTB_TUSS_ID = 5623
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5625 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.723' WHERE PROPTB_TUSS_ID = 5624
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5626 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.853' WHERE PROPTB_TUSS_ID = 5625
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5627 , CLOUD_SYNC_DATE = '2014-05-06 17:41:30.983' WHERE PROPTB_TUSS_ID = 5626
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5628 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.113' WHERE PROPTB_TUSS_ID = 5627
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5629 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.243' WHERE PROPTB_TUSS_ID = 5628
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5630 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.373' WHERE PROPTB_TUSS_ID = 5629
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5631 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.503' WHERE PROPTB_TUSS_ID = 5630
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5632 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.637' WHERE PROPTB_TUSS_ID = 5631
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5633 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.770' WHERE PROPTB_TUSS_ID = 5632
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5634 , CLOUD_SYNC_DATE = '2014-05-06 17:41:31.900' WHERE PROPTB_TUSS_ID = 5633
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5635 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.030' WHERE PROPTB_TUSS_ID = 5634
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5636 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.163' WHERE PROPTB_TUSS_ID = 5635
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5637 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.307' WHERE PROPTB_TUSS_ID = 5636
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5638 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.440' WHERE PROPTB_TUSS_ID = 5637
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5639 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.570' WHERE PROPTB_TUSS_ID = 5638
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5640 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.703' WHERE PROPTB_TUSS_ID = 5639
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5641 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.837' WHERE PROPTB_TUSS_ID = 5640
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5642 , CLOUD_SYNC_DATE = '2014-05-06 17:41:32.967' WHERE PROPTB_TUSS_ID = 5641
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5643 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.100' WHERE PROPTB_TUSS_ID = 5642
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5644 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.227' WHERE PROPTB_TUSS_ID = 5643
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5645 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.360' WHERE PROPTB_TUSS_ID = 5644
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5646 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.490' WHERE PROPTB_TUSS_ID = 5645
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5647 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.623' WHERE PROPTB_TUSS_ID = 5646
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5648 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.763' WHERE PROPTB_TUSS_ID = 5647
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5649 , CLOUD_SYNC_DATE = '2014-05-06 17:41:33.893' WHERE PROPTB_TUSS_ID = 5648
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5650 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.027' WHERE PROPTB_TUSS_ID = 5649
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5651 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.163' WHERE PROPTB_TUSS_ID = 5650
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5652 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.290' WHERE PROPTB_TUSS_ID = 5651
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5653 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.420' WHERE PROPTB_TUSS_ID = 5652
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5654 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.563' WHERE PROPTB_TUSS_ID = 5653
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5655 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.697' WHERE PROPTB_TUSS_ID = 5654
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5656 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.823' WHERE PROPTB_TUSS_ID = 5655
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5657 , CLOUD_SYNC_DATE = '2014-05-06 17:41:34.953' WHERE PROPTB_TUSS_ID = 5656
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5658 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.087' WHERE PROPTB_TUSS_ID = 5657
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5659 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.217' WHERE PROPTB_TUSS_ID = 5658
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5660 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.350' WHERE PROPTB_TUSS_ID = 5659
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5661 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.483' WHERE PROPTB_TUSS_ID = 5660
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5662 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.643' WHERE PROPTB_TUSS_ID = 5661
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5663 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.790' WHERE PROPTB_TUSS_ID = 5662
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5664 , CLOUD_SYNC_DATE = '2014-05-06 17:41:35.933' WHERE PROPTB_TUSS_ID = 5663
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5665 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.077' WHERE PROPTB_TUSS_ID = 5664
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5666 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.223' WHERE PROPTB_TUSS_ID = 5665
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5667 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.357' WHERE PROPTB_TUSS_ID = 5666
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5668 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.487' WHERE PROPTB_TUSS_ID = 5667
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5669 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.623' WHERE PROPTB_TUSS_ID = 5668
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5670 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.757' WHERE PROPTB_TUSS_ID = 5669
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5671 , CLOUD_SYNC_DATE = '2014-05-06 17:41:36.887' WHERE PROPTB_TUSS_ID = 5670
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5672 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.017' WHERE PROPTB_TUSS_ID = 5671
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5673 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.150' WHERE PROPTB_TUSS_ID = 5672
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5674 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.283' WHERE PROPTB_TUSS_ID = 5673
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5675 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.413' WHERE PROPTB_TUSS_ID = 5674
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5676 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.543' WHERE PROPTB_TUSS_ID = 5675
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5677 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.677' WHERE PROPTB_TUSS_ID = 5676
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5678 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.810' WHERE PROPTB_TUSS_ID = 5677
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5679 , CLOUD_SYNC_DATE = '2014-05-06 17:41:37.937' WHERE PROPTB_TUSS_ID = 5678
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5680 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.070' WHERE PROPTB_TUSS_ID = 5679
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5681 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.200' WHERE PROPTB_TUSS_ID = 5680
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5682 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.330' WHERE PROPTB_TUSS_ID = 5681
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5683 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.463' WHERE PROPTB_TUSS_ID = 5682
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5684 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.597' WHERE PROPTB_TUSS_ID = 5683
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5685 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.733' WHERE PROPTB_TUSS_ID = 5684
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5686 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.867' WHERE PROPTB_TUSS_ID = 5685
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5687 , CLOUD_SYNC_DATE = '2014-05-06 17:41:38.997' WHERE PROPTB_TUSS_ID = 5686
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5688 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.127' WHERE PROPTB_TUSS_ID = 5687
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5689 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.257' WHERE PROPTB_TUSS_ID = 5688
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5690 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.390' WHERE PROPTB_TUSS_ID = 5689
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5691 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.523' WHERE PROPTB_TUSS_ID = 5690
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5692 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.663' WHERE PROPTB_TUSS_ID = 5691
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5693 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.797' WHERE PROPTB_TUSS_ID = 5692
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5694 , CLOUD_SYNC_DATE = '2014-05-06 17:41:39.927' WHERE PROPTB_TUSS_ID = 5693
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5695 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.057' WHERE PROPTB_TUSS_ID = 5694
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5696 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.190' WHERE PROPTB_TUSS_ID = 5695
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5697 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.320' WHERE PROPTB_TUSS_ID = 5696
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5698 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.450' WHERE PROPTB_TUSS_ID = 5697
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5699 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.587' WHERE PROPTB_TUSS_ID = 5698
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5700 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.720' WHERE PROPTB_TUSS_ID = 5699
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5701 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.850' WHERE PROPTB_TUSS_ID = 5700
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5702 , CLOUD_SYNC_DATE = '2014-05-06 17:41:40.983' WHERE PROPTB_TUSS_ID = 5701
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5703 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.117' WHERE PROPTB_TUSS_ID = 5702
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5704 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.247' WHERE PROPTB_TUSS_ID = 5703
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5705 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.377' WHERE PROPTB_TUSS_ID = 5704
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5706 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.517' WHERE PROPTB_TUSS_ID = 5705
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5707 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.650' WHERE PROPTB_TUSS_ID = 5706
                                            UPDATE PROPTB_TUSS SET CLOUD_SYNC_ID = 5708 , CLOUD_SYNC_DATE = '2014-05-06 17:41:41.783' WHERE PROPTB_TUSS_ID = 5707");
                MessageBox.Show("Concluido");
            }

            catch
            {
                MessageBox.Show("Execute mais uma vez");

            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //DatabaseManager db = new DatabaseManager("Data Source=localhost\\SQLEXPRESS;Integrated Security=True;Connect Timeout=999;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=PersonalMed;");
            try
            {   // primeira parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE AGE011_PRECADASTRO SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE AGE02 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE AGE03 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //segunda parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE AGE04 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE AGE05 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE AMB90 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //terceira parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE AMB92 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE AMB96 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CARRIER SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //quarta parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CFG01 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CIEFAS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CLINI_01 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //quinta parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CLINI_02 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CLINI_05 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CO12_1 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //sexta parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CO12_2 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CO25 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CO28 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //setima parte
               ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CONTA_MATERIAIS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CONTA_MEDICAMENTOS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CONTA_PACIENTE SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //oitava parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CONTA_PACOTES SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CONTA_PROCEDIMENTOS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE CONTA_PROCEDIMENTOS_PROFISSIONAL SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //nona parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE CONTA_TAXAS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE FIN_02 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE FIN_51 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //decima parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE FUNCOES_REPASSES SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE LOCAL_ATENDIMENTO SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE ME SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //11 parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE PROPTB_TUSS SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE PROPTB59 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE PROPTB60 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //12 parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE PROPTB61 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE PROPTB62 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE PROPTB63 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //13 parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE PROPTB64 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE PROPTB65 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE SYS011 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //14 parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE SYS084 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE SYS099 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                UPDATE TISS009 SET CLOUD_SYNC_DATE = null, CLOUD_SYNC_ID = null;
                commit transaction;");
                //15 parte
                ConectDB().ExecuteNonQueries($@"
                begin transaction;
                UPDATE SYS103 SET CLOUD_SYNC_ID = null;
                UPDATE TUSS001_TUSS SET CLOUD_SYNC_ID = null;
                commit transaction;");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("Execute mais uma vez");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //DatabaseManager db = new DatabaseManager("Data Source=localhost\\SQLEXPRESS;Integrated Security=True;Connect Timeout=999;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Initial Catalog=PersonalMed;");
            try
            {
                ConectDB().ExecuteNonQueries($@"DELETE FROM RESUMO_PMED");
                ConectDB().ExecuteNonQueries($@"DELETE FROM CLINI_01_LOCK");
                MessageBox.Show("Concluido");

            }
            catch
            {
                MessageBox.Show("Execute mais uma vez");
            }
        }



        private void rbApagar_CheckedChanged(object sender, EventArgs e)
        {

            if (rbNaoApagar.Checked == false && rbApagar.Checked == true)
            {
                btApagaMedico.Enabled = true;

            }
            else if (rbNaoApagar.Checked == true && rbApagar.Checked == false)
            {
                btApagaMedico.Enabled = false;
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"
                DELETE FROM AGE03 WHERE AGE03_ID IN (
                select AGE03.AGE03_ID from age03 inner join
                (select data,inicio,usernumber, COUNT(*) AS C from AGE03
                where ATIVO = 'T' and ativo = 'F'
                group by DATA,INICIO,USERNUMBER
                having COUNT (*) > 1) duplicatas 

                on age03.DATA = duplicatas.DATA 
                AND AGE03.INICIO = duplicatas.INICIO 
                AND AGE03.USERNUMBER = duplicatas.USERNUMBER
                WHERE
                AGE03.ATIVO='T'and age03.ATIVO='F')");
                MessageBox.Show("Deletados");
            }
            catch (Exception)
            {
                MessageBox.Show("Execute mais uma vez");
            }
            finally
            {
                comboBox2.SelectedIndex = -1;
                AtualizarCombo();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
            try
            {
                ConectDB().ExecuteNonQueries($@"delete  from AGE04 where AGE03_ID not in (select AGE03_ID from AGE03)");
                MessageBox.Show("Deletados");
            }
            catch (Exception)
            {
                MessageBox.Show("Execute mais uma vez");
            }
            finally
            {
                comboBox2.SelectedIndex = -1;
                AtualizarCombo();
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            
            try
            {
               ConectDB().ExecuteNonQueries($@"update CLINI_01 set Clini_01_CPF = NULL 
    
               WHERE CLINI_01_cpf IN (    
    
               select CLINI_01.Clini_01_CPF from CLINI_01 inner join
    
               (select clini_01_cpf, COUNT(*) AS C from CLINI_01
               where ATIVO = 'T' 
               group by clini_01_cpf
               having COUNT (*) > 1) duplicatas 
               on CLINI_01.Clini_01_CPF = duplicatas.Clini_01_CPF 
               WHERE
               CLINI_01.ATIVO='T')");
                MessageBox.Show("Concluido com sucesso.");

            }
            catch (Exception)
            {
                MessageBox.Show("Execute mais uma vez");
            }
            finally
            {
                comboBox3.SelectedIndex = -1;
                comboBox4.SelectedIndex = -1;
                AtualizarCombo();
            }

        }

        private void btApagaMedico_Click(object sender, EventArgs e)
        {
            
            var profUserNumber = comboBox1.SelectedValue;
            try
            {
               ConectDB().ExecuteNonQueries($@"delete from GRUPO_USUARIO WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from PERMISSAO_USUARIO WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from SYS139_BACKUP where USERNUMBER in({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from US01 where USERNUMBER in({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from AGE04 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from AGE04 WHERE PROFESSIONALCOD IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from AGE06 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from AGE08 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from SYS011 WHERE PROFESSIONALCOD IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from CO19 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from CO22 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from CO28 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from ME WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from CLINI_02 WHERE USERNUMBER IN({profUserNumber})");

               ConectDB().ExecuteNonQueries($@"delete from CLINI_04 WHERE USERNUMBER IN({profUserNumber})");

                MessageBox.Show("Concluido com sucesso.");
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao e");

            }
            AtualizarCombo();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO19') AND NAME = 'CLOUD_SYNC_DATE')
                                            BEGIN
	                                            ALTER TABLE dbo.CO19 ADD CLOUD_SYNC_DATE DATETIME;
                                            END");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO50') AND NAME = 'CLOUD_SYNC_DATE')
                                                BEGIN
	                                                ALTER TABLE dbo.CO50 ADD CLOUD_SYNC_DATE DATETIME;
                                                END");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
               ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO19') AND NAME = 'CLOUD_SYNC_ID')
                                            BEGIN
	                                            ALTER TABLE dbo.CO19 ADD CLOUD_SYNC_ID INT;
                                            END");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");

            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO50') AND NAME = 'CLOUD_SYNC_ID')
                                                BEGIN
	                                                ALTER TABLE dbo.CO50 ADD CLOUD_SYNC_ID INT;
                                                END");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO51') AND NAME = 'CLOUD_SYNC_DATE')
                                                    BEGIN
	                                                    ALTER TABLE dbo.CO51 ADD CLOUD_SYNC_DATE DATETIME;
                                                    END

                                                     IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO51_1') AND NAME = 'CLOUD_SYNC_DATE')
                                                    BEGIN
	                                                    ALTER TABLE dbo.CO51_1 ADD CLOUD_SYNC_DATE DATETIME;
                                                    END ");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }

        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO51') AND NAME = 'CLOUD_SYNC_ID')
                                            BEGIN
	                                            ALTER TABLE dbo.CO51 ADD CLOUD_SYNC_ID INT;
                                            END

                                             IF NOT EXISTS(SELECT 1 FROM SYSCOLUMNS WHERE ID = OBJECT_ID('CO51_1') AND NAME = 'CLOUD_SYNC_ID')
                                            BEGIN
	                                            ALTER TABLE dbo.CO51_1 ADD CLOUD_SYNC_ID INT;
                                            END");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            try
            {
               ConectDB().ExecuteNonQueries($@"USE [PersonalMed]
                                        DROP TABLE DAT005

                                        USE [PersonalMed]
                                        GO
                                        SET ANSI_NULLS ON
                                        GO

                                        SET QUOTED_IDENTIFIER ON
                                        GO

                                        SET ANSI_PADDING ON
                                        GO

                                        CREATE TABLE [dbo].[DAT005](
	                                        [TABLENAME] [varchar](20) NOT NULL,
	                                        [NEXTKEY] [int] NOT NULL,
	                                        [TABLE] [varchar](255) NULL,
	                                        [FIELD] [varchar](255) NULL,
                                         CONSTRAINT [PK__DAT005__7E4D98E6] PRIMARY KEY CLUSTERED 
                                        (
	                                        [TABLENAME] ASC
                                        )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                                        ) ON [PRIMARY]

                                        GO

                                        SET ANSI_PADDING OFF
                                        GO");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"USE [PersonalMed]
                                        GO
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE01', 1, N'AGE01', N'AGE01_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE011_PRECADASTRO', 1, N'AGE011_PRECADASTRO', N'AGE011_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE012', 1, N'AGE012_RECURRENCE', N'AGE012_RECURRENCE_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE013', 1, N'AGE013_EXCEPTION_RECURRENCE', N'AGE013_EXCEPTION_RECURRENCE_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE014_CANCELAMENTO', 2, N'AGE014_MOTIVOSCANCELAMENTOS', N'AGE014_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE02', 1, N'AGE02', N'AGE02_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE03', 1, N'AGE03', N'AGE03_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE04', 1, N'AGE04', N'AGE04_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE05', 1, N'AGE05', N'AGE05_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE09', 1, N'AGE09', N'PACOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGE10', 1, N'AGE10', N'PROCEDUREWEBID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AGFONE', 1, N'AGFONE', N'AGENDAFONECODE')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_001', 1, N'AUD_001', N'CODEXAMEAUDIO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_002', 1, N'AUD_002', N'CODAPARELHO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_005', 1, N'AUD_005', N'CODEPI')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_007', 1, N'AUD_007', N'AUD_007_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_008', 1, N'AUD_008', N'AUD_008_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'AUD_009', 1, N'AUD_009', N'AUD_009_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_006', 1, N'CAR_006', N'DOCTORCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_007', 1, N'CAR_007', N'HOSPITALCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_008', 1, N'CAR_008', N'SURGERYCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_012', 1, N'CAR_012', N'COMPLICATIONCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_013', 1, N'CAR_013', N'OBITCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_014', 253, N'CAR_014', N'TEAMPROCEEDINGCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CAR_015', 289, N'CAR_015', N'TEAMDIAGNOSECOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CARRIER', 1, N'CARRIER', N'CARRIERCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_02', 1, N'CLINI_02', N'CLINI_02_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_04', 1, N'CLINI_04', N'IMAGECOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_05', 1, N'CLINI_05', N'CLINI_05_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_06', 1, N'CLINI_06', N'IMPNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_06_REC_ITEM', 1, N'CLINI_06_RECEITA_ITEM', N'ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_08', 1, N'CLINI_08', N'CLINI_08_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_10', 1, N'CLINI_10', N'CLINI_10_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_12', 1, N'CLINI_12', N'CLINI_12_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_18', 1, N'CLINI_18_PROBLEM_PATIENT', N'CLINI_18_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_19', 1, N'CLINI_19_SYMPTOM_PATIENT', N'CLINI_19_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_20', 1, N'CLINI_20_EVENT_PATIENT', N'CLINI_20_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_21', 1, N'CLINI_21_PROCEDURE_PATIENT', N'CLINI_21_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CLINI_22', 1, N'CLINI_22', N'CLINI_22_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO050001', 1, N'CO050001', N'OCCUPATIONALEXAMCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO050002', 1, N'CO050002', N'CO050002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO050003', 1, N'CO050003', N'CO050003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO050004', 1, N'CO050004', N'CO050004_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO053001', 1, N'CO053001', N'ABSCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO059001', 1, N'CO059001', N'CODASO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO059002', 1, N'CO059002', N'CO059002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO059003', 1, N'CO059003', N'CO059003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO059004', 1, N'CO059004', N'CO059004_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO059011', 1, N'CO059011', N'CODCAT')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO12_1', 1, N'CO12_1', N'CODIGO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO12_2', 1, N'CO12_2', N'CO12_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO17', 1, N'CO17', N'EXAMNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO17DPES', 1, N'CO17DPES', N'CO17DPES_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO18', 1, N'CO18', N'CO18_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO19', 1, N'CO19', N'CONSNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO20', 1, N'CO20', N'EXAMNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO22', 1, N'CO22', N'CO22_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO22_1', 1, N'CO22_1', N'CO22_1_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO22_3', 1, N'CO22_3', N'CO22_3_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO25', 1, N'CO25', N'PROCNUMB')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO28', 1, N'CO28', N'CO28_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO31', 1, N'CO31', N'EXAMNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO33', 1, N'CO33', N'CO33_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO33_1', 1, N'CO33_1', N'CO33_1_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO34', 1, N'CO34', N'CODIGO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO43', 1, N'CO43', N'CODCIRURGIA')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO44_1', 1, N'CO44_1', N'CODPSA')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO44_2', 1, N'CO44_2', N'CODEQU')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO45', 1, N'CO45', N'CODASP')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO46', 1, N'CO46', N'CODANAMNESE')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO47', 1, N'CO47', N'CODEXAME')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO48', 1, N'CO48', N'CODEXAME')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO49_1', 1, N'CO49_1', N'NUMBERGESTATION')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO49_2', 1, N'CO49_2', N'NUMBERCONSULTATION')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO50', 1, N'CO50', N'CO50_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO51_1', 1, N'CO51_1', N'CO51_1_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO52', 1, N'CO52', N'CO52_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO53', 1, N'CO53', N'CODIGO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO53_2', 1, N'CO53_2', N'CO53_2_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO55', 1, N'CO55', N'CODEXAME')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO62', 1, N'CO62', N'CO62_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO62_1', 1, N'CO62_1', N'CO62_1_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'CO63', 1, N'CO63', N'CO63_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DAT001', 10039, N'DAT001', N'RISKCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DIAGPESS', 1, N'DIAGPESS', N'DIAGCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DMED_CONFIGURACAO', 2, N'DMED_CONFIGURACAO', N'CFG_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DMED_DECLARACAO', 1, N'DMED_DECLARACAO', N'DEC_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DMED_DEPENDENTE', 1, N'DMED_DEPENDENTE', N'DEP_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'DMED_FINANCEIRO', 1, N'DMED_FINANCEIRO', N'FINAN_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT001', 1, N'FAT001_PERCENT_PARTICIPATION', N'FAT001_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT002', 1, N'FAT002_PACKAGE_MEDICINE', N'FAT002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT003', 1, N'FAT003_PACKAGE_MATERIAL', N'FAT003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT004', 1, N'FAT004_PACKAGE_TAXES', N'FAT004_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT005', 1, N'FAT005_PROCEDURE_EXCEPTION', N'FAT005_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT006', 1, N'FAT006_REPASS_PERCENT', N'FAT006_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT007', 1, N'FAT007_TISSTYPE_REPASS_CONFIG', N'FAT007_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT008', 1, N'FAT008_REPASS', N'FAT008_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT009', 1, N'FAT009_TYPE_REPASS', N'FAT009_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT010', 1, N'FAT010_ITEM_REPASS', N'FAT010_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT011', 1, N'FAT011_HOLIDAY', N'FAT011_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT012', 1, N'FAT012_DEFAULT_ASSISTANT', N'FAT012_ID')
                                        GO
                                        print 'Processed 100 total records'
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT013', 1, N'FAT013_ANESTHETIC_TABLE', N'FAT013_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FAT014', 1, N'FAT014_DEFAULT_ANESTHETIC', N'FAT014_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FIN_17', 2, N'FIN_17', N'PROCEDURECOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FIN_31', 1, N'FIN_31', N'ACCOUNTNUMBER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FIN_51', 2, N'FIN_51', N'CARRIERGROUPCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FIN_65', 1, N'FIN_65', N'FIN_65_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'FIN_66', 1, N'FIN_66', N'FIN_66_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'GRUPO', 6, N'GRUPO', N'GRU_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'GRUPO_PERFIL', 6, N'GRUPO_PERFIL', N'GRP_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'GRUPO_USUARIO', 1, N'GRUPO_USUARIO', N'GUS_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'INT001', 1, N'INT001_PATIENT_EXAM_RESULT', N'INT001_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'LABEXAM', 156, N'LABEXAM', N'LEXAMCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'LABS', 1, N'LABS', N'LABCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'LOT001', 1, N'LOT001', N'LOT001_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'LOT002', 1, N'LOT002', N'LOT002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'ME', 1, N'ME', N'ME_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'OPERACAO', 7, N'OPERACAO', N'OPE_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO005', 1, N'PCMSO005', N'PCMSO005_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO006', 1, N'PCMSO006', N'PCMSO006_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO007', 1, N'PCMSO007', N'PCMSO007_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO008', 1, N'PCMSO008', N'PCMSO008_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO011', 1, N'PCMSO011', N'PCMSO011_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO012', 1, N'PCMSO012', N'PCMSO012_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO013', 1, N'PCMSO013', N'PCMSO013_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO014', 40, N'PCMSO014', N'PCMSO014_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO015', 1, N'PCMSO015', N'PCMSO015_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO016', 1, N'PCMSO016', N'PCMSO016_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PCMSO018', 1, N'PCMSO018', N'PCMSO018_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PERFIL', 6, N'PERFIL', N'PER_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PERGUNTA_SECRETA', 19, N'PERGUNTA_SECRETA', N'IDPERGUNTA')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PERMISSAO_PERFIL', 6743, N'PERMISSAO_PERFIL', N'PEP_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PERMISSAO_USUARIO', 1, N'PERMISSAO_USUARIO', N'PEC_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP001', 1, N'PPP001', N'CODPERFIL')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP002', 1, N'PPP002', N'CODPERFILCAT')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP003', 1, N'PPP003', N'CODPERFILLOTATRIB')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP004', 1, N'PPP004', N'CODPERFILPROFISSIOGRAFIA')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP005', 1, N'PPP005', N'CODPERFILRISCO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP006', 1, N'PPP006', N'CODPERFILRESPRISCO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP007', 1, N'PPP007', N'CODPERFILEXAME')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP008', 1, N'PPP008', N'CODPERFILRESPEXAME')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPP013', 1, N'PPP013', N'PPPEPQUESTIONCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPRA069', 6, N'PPRA069', N'QUESTIONCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PPRA070', 1, N'PPRA070', N'EPQUESTIONCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB_TUSS', 5792, N'PROPTB_TUSS', N'PROPTB_TUSS_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB59', 1, N'PROPTB59', N'PROPTB59_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB60', 1, N'PROPTB60', N'PROPTB60_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB61', 1, N'PROPTB61', N'PROPTB61_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB62', 1, N'PROPTB62', N'PROPTB62_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB63', 1, N'PROPTB63', N'PROPTB63_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB64', 1, N'PROPTB64', N'PROPTB64_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROPTB65', 1, N'PROPTB65', N'PROPTB65_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'PROT_AGE_CANCELA', 1, N'PROT_AGE_CANCELA', NULL)
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'RECURSO', 1027, N'RECURSO', N'REC_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'RM$CONTACAIXA', 2, N'RM$CONTACAIXA', N'RM$ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SESSAO', 1, N'SESSAO', N'SES_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'STO_03', 1, N'STO_03', N'FURNISHERCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'STO_06', 1, N'STO_06', N'IDENTIFIER')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'STO_07', 1, N'STO_07', N'MOTIVECOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS001', 1, N'SYS001', N'ENTERPRISECOD
                                        ')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS007', 4, N'SYS007', N'SITUATIONCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS008', 4, N'SYS008', N'CATEGCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS035', 1, N'SYS035', N'DOCTORCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS065', 1, N'SYS065', N'DOCTORCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS066', 1, N'SYS066', N'LABORSECURITYCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS067', 1, N'SYS067', N'LEGALREPRESCOD')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS071', 1, N'SYS071', N'CODESPEC')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS101', 1986, N'SYS101', N'SYS101_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS105', 1, N'SYS105', N'SYS105_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS115', 358, N'SYS115_COMMENT', N'SYS115_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS119', 1, N'SYS119_PROBLEM', N'SYS119_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS120', 1, N'SYS120_ASSOCIATED_PROBLEM', N'SYS120_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS121', 1, N'SYS121_SYMPTOM', N'SYS121_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS122', 1, N'SYS122_SYMPTOM_PROBLEM', N'SYS122_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS123', 1, N'SYS123_EVENT', N'SYS123_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS124', 1, N'SYS124_EVENT_PROBLEM', N'SYS124_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS125', 1, N'SYS125_PROCEDURE', N'SYS125_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS126', 1, N'SYS126_PROCEDURE_PROBLEM', N'SYS126_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS127', 18, N'SYS127_WS_CONFIG', N'SYS127_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS127_WS_CONFIG', 2, NULL, NULL)
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS128', 26, N'SYS128_WS_PARAMETER', N'SYS128_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS130', 1, N'SYS130_GROUP_EXAM', N'SYS130_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS131', 1, N'SYS131_GROUP_COMPLAINT', N'SYS131_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'SYS139', 1, N'SYS139_BACKUP', N'SYS139_BACKUP_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS_NUMERO_LOTE', 1, NULL, NULL)
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS_TRANSACAO_GUIA', 1, NULL, NULL)
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS001', 1, N'TISS001', N'TISS001_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS002', 1, N'TISS002', N'TISS002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS003', 1, N'TISS003', N'TISS003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS004', 1, N'TISS004', N'TISS004_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS005', 1, N'TISS005', N'TISS005_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS006', 1, N'TISS006', N'TISS006_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS007', 1, N'TISS007', N'TISS007_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS008', 1, N'TISS008', N'TISS008_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS009', 1, N'TISS009', N'TISS009_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS011', 1, N'TISS011', N'TISS011_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS012', 1, N'TISS012', N'TISS012_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS013', 1, N'TISS013', N'TISS013_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS014', 1, N'TISS014', N'TISS014_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS015', 1, N'TISS015', N'TISS015_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS016', 1, N'TISS016', N'TISS016_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS017', 1, N'TISS017', N'TISS017_ID')
                                        GO
                                        print 'Processed 200 total records'
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS018', 1, N'TISS018', N'TISS018_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS019', 1, N'TISS019', N'TISS019_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS020', 1, N'TISS020', N'TISS020_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS021', 1, N'TISS021', N'TISS021_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS022', 1, N'TISS022', N'TISS022_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS023', 1, N'TISS023', N'TISS023_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS024', 1, N'TISS024', N'TISS024_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS025', 1, N'TISS025', N'TISS025_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS026', 1, N'TISS026_GUIDE_COMMENT', N'TISS026_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS027', 1, N'TISS027_COMMENT_PROC_SADT', N'TISS027_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS028', 1, N'TISS028_COMMENT_PROC_RESUME', N'TISS028_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS029', 1, N'TISS029_COMMENT_PROC_HONORARY', N'TISS029_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS030', 1, N'TISS030_COMMENT_PROC_CONSULT', N'TISS030_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS031', 1, N'TISS031_COMMENT_OPM_SADT', N'TISS031_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS032', 1, N'TISS032_COMMENT_OPM_RESUME', N'TISS032_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS033', 1, N'TISS033_COMMENT_OTHEREXPENSES_SADT', N'TISS033_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS034', 1, N'TISS034_COMMENT_OTHEREXPENSES_RESUME', N'TISS034_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS035', 1, N'TISS035_DEMONSTRATIVE', N'TISS035_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS036', 1, N'TISS036_RESUME_DEMONSTRATIVE', N'TISS036_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS037', 1, N'TISS037_DEMONSTRATIVE_OFF', N'TISS037_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS038', 1, N'TISS038_ANALYSIS_BILL', N'TISS038_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS039', 1, N'TISS039_INVOICE_GUIDE', N'TISS039_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS040', 1, N'TISS040_INVOICE_PROC_SADT', N'TISS040_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS041', 1, N'TISS041_INVOICE_PROC_RESUME', N'TISS041_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS042', 1, N'TISS042_INVOICE_PROC_HONORARY', N'TISS042_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS043', 1, N'TISS043_INVOICE_PROC_CONSULT', N'TISS043_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS044', 1, N'TISS044_INVOICE_OPM_SADT', N'TISS044_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS045', 1, N'TISS045_INVOICE_OTHER_EXPENSES_SADT', N'TISS045_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS046', 1, N'TISS046_INVOICE_OPM_RESUME', N'TISS046_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS047', 1, N'TISS047_INVOICE_OTHER_EXPENSES_RESUME', N'TISS047_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TISS048', 1, N'TISS048_COMMENT_PROC_REQUEST_INTERN', N'TISS048_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_000', 1, N'TRA_000', N'TRA_000_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_001', 1, N'TRA_001', N'TRA_001_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_002', 1, N'TRA_002', N'TRA_002_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_003', 1, N'TRA_003', N'TRA_003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_004', 1, N'TRA_004', N'TRA_004_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_005', 1, N'TRA_005', N'TRA_005_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_006', 1, N'TRA_006', N'TRA_006_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_007', 1, N'TRA_007', N'TRA_007_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_008', 1, N'TRA_008', N'TRA_008_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TRA_009', 1, N'TRA_009', N'TRA_009_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TUSS001', 5809, N'TUSS001_TUSS', N'TUSS001_TUSS_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TUSS002', 5708, N'TUSS002_CORRELATION', N'TUSS002_CORRELATION_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'TUSS003', 72662, N'TUSS003_FROM_TO_TUSS', N'TUSS003_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'US04', 1, N'US04', N'US04_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'US05_RECEITA_ITEM', 1, N'US05_RECEITA_ITEM', N'ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'US06', 1, N'US06', N'US06_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'US07', 1, N'US07', N'US07_ID')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'us08', 1, N'us08', N'us08_id')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'VAC_CFG', 32, N'VAC_CFG', N'CODIGO')
                                        INSERT [dbo].[DAT005] ([TABLENAME], [NEXTKEY], [TABLE], [FIELD]) VALUES (N'Val_Norm', 137, N'Val_Norm', N'titlecod')
                                        ");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"DECLARE @TABLENAME VARCHAR(255);
                                            DECLARE @FIELDNAME VARCHAR(255);
                                            DECLARE @NEXTKEY INT;
                                            DECLARE @SQLString NVARCHAR(255);
                                            DECLARE @ParmDefinition NVARCHAR(255);

                                            DECLARE CUR_TEMP CURSOR FOR
                                            SELECT  TABLENAME, FIELD
                                            FROM DAT005 d
                                            INNER JOIN DBO.SYSOBJECTS  s
                                               ON (s.ID = OBJECT_ID(d.TABLENAME)) AND (OBJECTPROPERTY(s.ID, N'ISUSERTABLE') = 1)
                                            WHERE TABLENAME IS NOT NULL
                                              AND FIELD IS NOT NULL
                                            ORDER BY TABLENAME

                                            OPEN CUR_TEMP
                                            FETCH NEXT FROM CUR_TEMP INTO @TABLENAME, @FIELDNAME
                                            WHILE (@@FETCH_STATUS = 0)
                                            BEGIN
	                                            SET @SQLString = N'select @NEXTKEY_OUT = COALESCE(max('+ (@FIELDNAME) +') + 1, 1) from [' + @TABLENAME +']';
	                                            SET @ParmDefinition = N'@NEXTKEY_OUT int OUTPUT';
	                                            EXECUTE sp_executesql @SQLString, @ParmDefinition, @NEXTKEY_OUT = @NEXTKEY OUTPUT;
	     
	                                            UPDATE DAT005
  	                                               SET NEXTKEY = @NEXTKEY
                                                     WHERE TABLENAME = @TABLENAME

	                                            FETCH NEXT FROM CUR_TEMP INTO @TABLENAME, @FIELDNAME
                                            END
                                            CLOSE CUR_TEMP
                                            DEALLOCATE CUR_TEMP");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {
                MessageBox.Show("NÃO EXECUTADO!");
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"UPDATE CLINI_01 SET clini_01_cpf = REPLACE(REPLACE(REPLACE(clini_01_cpf,'.',''),'-',''), ' ', '')
                                                where clini_01_cpf is not null
                                            ");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {

                MessageBox.Show("NÃO EXECUTADO!"); 
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                ConectDB().ExecuteNonQueries($@"update clini_01  set Clini_01_CPF = NULL
                                            WHERE LEN(Clini_01_CPF) > 11");
                MessageBox.Show("Concluido");
            }
            catch (Exception)
            {

                MessageBox.Show("");
                MessageBox.Show("NÃO EXECUTADO!");
            }
            
        }
    }

}

