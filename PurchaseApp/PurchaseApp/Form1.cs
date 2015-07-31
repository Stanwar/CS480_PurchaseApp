using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PurchaseApp
{
    public partial class Form1 : Form
    {
        string connectionInfo;
        string typeSelection;
        //
        // uiTunes music purchase application
        //
        // Sharad Tanwar
        // U. of Illinois, Chicago
        // CS480, Summer 2015
        // Homework 3
        //
        public Form1()
        {
            InitializeComponent();

            // 
            // constructor: setup connection info
            //
            string filename = "uitunes.mdf";

            connectionInfo = String.Format(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\{0};Integrated Security=True;", filename);
            //this.button1.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = false;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            typeSelection = "ALBUMS";
            SqlConnection db;
            db = new SqlConnection(connectionInfo);
            db.Open();

            // MessageBox.Show(db.State.ToString());
            string sql, msg;
            SqlCommand cmd;
            object result;
            sql = String.Format("select AlbumName from Albums Order by AlbumName ASC");

            cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = sql;

            //result = cmd.ExecuteScalar();

            //msg = String.Format("Song id : {0}", result);

            //listBox1.Items.Add(msg);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            //
            adapter.Fill(ds);
            //
            DataTable dt = ds.Tables["TABLE"];
            //
            this.listBox1.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {

                msg = string.Format("{0}",
                row["AlbumName"].ToString()
                );

                listBox1.Items.Add(msg);
            }

            db.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            SqlConnection db;
            db = new SqlConnection(connectionInfo);
            db.Open();


            // MessageBox.Show(db.State.ToString());
            string sql, msg;
            SqlCommand cmd;
            object result;
            string textbox1 = this.textBox1.Text;
            textbox1 = textbox1.Replace("'","''");
            /////
            string textbox2 = this.textBox2.Text;
            textbox2 = textbox2.Replace("'", "''");
            /////
            string textbox3 = this.textBox3.Text;
            textbox3 = textbox3.Replace("'", "''");
            /////
            sql = String.Format(@"Select 
                                 AcctBal,
	                             (CASE 
	                             WHEN AcctBal > 0 THEN 'CREDIT'
	                             WHEN AcctBal <=0 THEN 'DEBIT'
	                             END) Account_Status
                              from users
                              Where FirstName = '{0}' 
                                AND LastName = '{1}'
	                            AND Pwd = '{2}';",
                     textbox1, 
                     textbox2,
                     textbox3);
            /////

            //sql = String.Format("select songID from Songs Where Songname = '{0}';", this.textBox1.Text);
            //MessageBox.Show(sql);
            cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = sql;

            result = cmd.ExecuteScalar();
            this.textBox4.Clear();
            if(result != null ){
                //this.button1.Enabled = true;
                this.button2.Enabled = true;
                this.button3.Enabled = true;
                //this.button4.Enabled = true;
            }
            else
            {
                MessageBox.Show("Invalid FirstName or LastName or Password");
            }
            // Clearing the textbox
            
            msg = String.Format("AcctBal : {0}", result);
            
            MessageBox.Show(msg);
            this.textBox4.AppendText(msg);
            db.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            typeSelection = "SONGS";
            SqlConnection db;
            db = new SqlConnection(connectionInfo);
            db.Open();

            // MessageBox.Show(db.State.ToString());
            string sql, msg;
            SqlCommand cmd;
            object result;
            sql = String.Format("select SongName from Songs Order by Songname ASC");
            
            cmd = new SqlCommand();
            cmd.Connection = db;
            cmd.CommandText = sql;

            //result = cmd.ExecuteScalar();

            //msg = String.Format("Song id : {0}", result);

            //listBox1.Items.Add(msg);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataSet ds = new DataSet();
            //
            adapter.Fill(ds);
            //
            DataTable dt = ds.Tables["TABLE"];
            //
            this.listBox1.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {

                msg = string.Format("{0}",
                row["SongName"].ToString()
                );

                listBox1.Items.Add(msg);
            }

            db.Close();

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SqlConnection db;
            db = new SqlConnection(connectionInfo);
            db.Open();
            this.button4.Enabled = true;
            // MessageBox.Show(db.State.ToString());
            string sql, msg;
            SqlCommand cmd;
            object result;
            if(typeSelection == "SONGS"){

                string listbox1 = this.listBox1.Text;
                listbox1 = listbox1.Replace("'", "''");
                ////
                sql = String.Format(@"
                       SELECT A.ArtistName, 
                                C.YearRel, 
	                            C.Price,	   
			                    (CASE WHEN (R.Total_No_Reviews ) IS NOT NULL 
					                    THEN R.Total_No_Reviews
					                    ELSE '0' 
					                    END )Total_No_Reviews,
			                    (CASE WHEN R.Average_Rating IS NOT NULL 
				                    THEN R.Average_Rating
				                    ELSE 'N/A'
				                    END ) Average_Rating
                            FROM Artists A, 
                                SongDetails B, 
	                            Songs C
			                    LEFT JOIN 
	                            (SELECT ReviewItemID,
	                                    COUNT(ReviewItemID) Total_No_Reviews, 
	                                    CAST(ROUND(AVG(CAST(Rating AS Float)), 4) AS nvarchar) Average_Rating
	                                FROM Reviews R
		                            WHERE R.ReviewItemTypeID = 1
		                            GROUP BY ReviewItemID) R
				                    ON  R.ReviewItemID = C.SongID
                            WHERE A.ArtistID = B.ArtistID
                            AND B.SongID = C.SongID
                            AND C.SongName = '{0}';", listbox1);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;

                //MessageBox.Show(sql);
                //result = cmd.ExecuteScalar();

                //msg = String.Format("Song id : {0}", result);

                //listBox1.Items.Add(msg);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                //
                adapter.Fill(ds);
                //
                DataTable dt = ds.Tables["TABLE"];
                //
                this.listBox2.Items.Clear();
                foreach (DataRow row in dt.Rows)
                {

                    msg = string.Format(" Artist Name :{0}, YearRel:{1},Price:{2},Total Reviews:{3},Average Rating:{4}",
                   row["ArtistName"].ToString(),
                   row["YearRel"].ToString(),
                   row["Price"].ToString(),
                   row["Total_No_Reviews"].ToString(),
                   row["Average_Rating"].ToString()
                   );
                    //MessageBox.Show(msg);
                    this.listBox2.Items.Add(msg);
                }

            }
            else if (typeSelection == "ALBUMS"){
                string listbox1 = this.listBox1.Text;
                listbox1 = listbox1.Replace("'","''");
                sql = string.Format(@"SELECT A.ArtistName, 
                                            C.YearRel, 
	                                        c.Price,
	                                        (CASE WHEN (R.Total_No_Reviews ) IS NOT NULL 
		                                         THEN R.Total_No_Reviews
			                                     ELSE '0' 
			                                     END )Total_No_Reviews,
	                                        (CASE WHEN R.Average_Rating IS NOT NULL 
		                                        THEN R.Average_Rating
			                                    ELSE 'N/A'
			                                    END ) Average_Rating
                                        FROM Artists A, -- Inner Join 
	                                        AlbumDetails B, -- Inner Join
	                                        Albums C
                                            LEFT JOIN (
                                                    SELECT ReviewItemID,
	                                                COUNT(ReviewItemID) Total_No_Reviews, 
	                                                CAST (ROUND(AVG(CAST(Rating AS Float)), 4) AS nvarchar) Average_Rating
	                                            FROM Reviews R
		                                        WHERE R.ReviewItemTypeID = 2
		                                        GROUP BY ReviewItemID
                                            ) R	
		                                    ON R.ReviewItemID = C.AlbumID  
                                        WHERE A.ArtistID = B.ArtistID
                                        AND B.AlbumID = C.AlbumID
                                        AND C.AlbumName = '{0}';", listbox1);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;
                //MessageBox.Show(sql);
                //result = cmd.ExecuteScalar();

                //msg = String.Format("Song id : {0}", result);

                //listBox1.Items.Add(msg);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataSet ds = new DataSet();
                //
                adapter.Fill(ds);
                //
                DataTable dt = ds.Tables["TABLE"];
                //
                this.listBox2.Items.Clear();
                //
                foreach (DataRow row in dt.Rows)
                {

                    msg = string.Format("Artist Name : {0},YearRel : {1}, Price : {2},Total Reviews : {3},Average Rating: {4}",
                    row["ArtistName"].ToString(),
                    row["YearRel"].ToString(),
                    row["Price"].ToString(),
                    row["Total_No_Reviews"].ToString(),
                    row["Average_Rating"].ToString()
                    );

                    //MessageBox.Show(msg);
                    this.listBox2.Items.Add(msg);
                }

            
            }
            

            db.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlConnection db;
            db = new SqlConnection(connectionInfo);
            db.Open();


            // MessageBox.Show(db.State.ToString());
            string sql,sql2, msg;
            SqlCommand cmd;
            object result, result2;

            if (typeSelection == "SONGS"){
                string textbox2 = textBox2.Text, textbox1= textBox1.Text,textbox3= textBox3.Text, listbox1 = listBox1.Text;
                
                textbox1 = textbox1.Replace("'","''");
                textbox2 = textbox2.Replace("'","''");
                textbox3 = textbox3.Replace("'","''");
                listbox1 = listbox1.Replace("'","''");

                sql = string.Format(@"Declare @UserID As Integer;
                                Declare @SongID As Integer;

                                SELECT @UserID = UserID 
                                  FROM Users 
                                 WHERE LastName = '{0}' 
                                   and FirstName = '{1}'
                                   and Pwd  = {2}
                                   ;

                                If @UserID IS NULL  -- not found, error?
                                  Set @UserID = NULL;  -- do nothing
                                Else Begin
                                  DECLARE @temp As Integer;

                                  SELECT @SongID = SongID 
                                  FROM Songs
                                 WHERE SongName = '{3}'
                                ;

                                SELECT P.PurchaseID
                                  FROM Purchases P
                                  INNER JOIN (SELECT P1.* 
                                                FROM PurchaseDetails P1
				                                INNER JOIN(SELECT * 
				                                             FROM PurchaseItemTypes
						                                  ) P2
			                                  ON P1.PurchaseItemTypeID  = P2.PurchaseItemTypeID
			                                  WHERE UPPER(P2.PurchaseItemTypeName) = UPPER('Song')
			                                  ) T
                                 ON P.PurchaseID = T.PurchaseID
                                 WHERE  P.UserID = @UserID 
                                 AND T.PurchaseItemID = @SongID;
                                End;", textbox2, textbox1, textbox3, listbox1);
                //MessageBox.Show(sql);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;

                result = cmd.ExecuteScalar();
                // Clearing the Textbox
                this.textBox4.Clear();

                if (result == null)
                {
                    //MessageBox.Show("This item has not been purchased before.");
                    this.textBox4.AppendText("This item has not been purchased before.");

                    sql2 = string.Format(@"Declare @UserID As Integer;
                                        Declare @Acct As Integer;
                                        Declare @Price As Float;
                                        Declare @SongID As Integer;
                                        Declare @MaxPurchase As Integer;
                                        Declare @purchaseID As Integer;

                                        SELECT @UserID = UserID ,
	                                           @Acct = AcctBal
                                          FROM Users 
                                         WHERE LastName = '{0}' 
                                           and FirstName = '{1}'
                                           and Pwd  = {2}
                                           ;

                                        If @UserID IS NULL  -- not found, error?
                                          Set @UserID = NULL;  -- do nothing
                                        Else 
                                            Begin
                                          DECLARE @temp As Integer;

                                          SELECT @SongID = SongID ,
                                                 @Price = Price 
                                            FROM Songs
                                           WHERE SongName = '{3}'
                                        ;

                                        
                                        if (@Price > @Acct)
	                                        Select -1;
                                        else 
	                                        UPDATE Users
	                                           SET AcctBal = AcctBal - @Price
	                                          WHERE UserID = @UserID
	                                          ; 
	                                        INSERT INTO Purchases(UserID, AmountPaid, PurchaseDate) 
	                                             VALUES(@UserID,@Price,CAST(GETDATE() AS DATE));
	                                        
                                            -- SELECT @MaxPurchase = MAX(PurchaseID) FROM Purchases;
                                            Set @purchaseID = @@IDENTITY;
	                                        
                                               INSERT INTO PurchaseDetails(PurchaseID,PurchaseItemTypeID,PurchaseItemID) 
	                                             VALUES(@purchaseID,1,@SongID);
                                        End;", textbox2, textbox1, textbox3, listbox1);
                    //MessageBox.Show(sql2);
                    //MessageBox.Show(sql);

                    cmd.CommandText = sql2;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show(rowsModified.ToString());
                    if (rowsModified <3)
                    {
                        this.textBox4.AppendText("Not Enough Balance. Please add value");
                    }
                    else
                    {
                        this.textBox4.AppendText("Item was successfully bought");

                    }

                }

                else
                {
                    //MessageBox.Show("This item has been purchased before");
                    this.textBox4.AppendText("This item has been purchased before");
                }
            }
            else if (typeSelection == "ALBUMS"){
                string textbox2 = textBox2.Text, textbox1 = textBox1.Text, textbox3 = textBox3.Text, listbox1 = listBox1.Text;
                textbox1 = textbox1.Replace("'", "''");
                textbox2 = textbox2.Replace("'", "''");
                textbox3 = textbox3.Replace("'", "''");
                listbox1 = listbox1.Replace("'", "''");
                
                sql = string.Format(@"Declare @UserID As Integer;
                                Declare @AlbumID As Integer;

                                SELECT @UserID = UserID 
                                  FROM Users 
                                 WHERE LastName = '{0}' 
                                   and FirstName = '{1}'
                                   and Pwd  = {2}
                                   ;

                                If @UserID IS NULL  -- not found, error?
                                  Set @UserID = NULL;  -- do nothing
                                Else Begin
                                  DECLARE @temp As Integer;

                                  SELECT @AlbumID = AlbumID 
                                  FROM Albums
                                 WHERE AlbumName = '{3}'
                                ;

                                SELECT P.PurchaseID
                                  FROM Purchases P
                                  INNER JOIN (SELECT P1.* 
                                                FROM PurchaseDetails P1
				                                INNER JOIN(SELECT * 
				                                             FROM PurchaseItemTypes
						                                  ) P2
			                                  ON P1.PurchaseItemTypeID  = P2.PurchaseItemTypeID
			                                  WHERE UPPER(P2.PurchaseItemTypeName) = UPPER('Album')
			                                  ) T
                                 ON P.PurchaseID = T.PurchaseID
                                 WHERE  P.UserID = @UserID 
                                 AND T.PurchaseItemID = @AlbumID;
                                End;", textbox2, textbox1, textbox3, listbox1);
                //MessageBox.Show(sql);

                cmd = new SqlCommand();
                cmd.Connection = db;
                cmd.CommandText = sql;

                result2 = cmd.ExecuteScalar();
                // Clearing the Textbox
                this.textBox4.Clear();

                if (result2 == null)
                {
                    //MessageBox.Show("This item has not been purchased before.");
                    this.textBox4.AppendText("This item has not been purchased before.");

                    sql2 = string.Format(@"Declare @UserID As Integer;
                                        Declare @Acct As Integer;
                                        Declare @Price As Float;
                                        Declare @AlbumID As Integer;
                                        Declare @MaxPurchase As Integer;
                                        Declare @purchaseID AS Integer;

                                        SELECT @UserID = UserID ,
	                                           @Acct = AcctBal
                                          FROM Users 
                                         WHERE LastName = '{0}' 
                                           and FirstName = '{1}'
                                           and Pwd  = {2}
                                           ;

                                        If @UserID IS NULL  -- not found, error?
                                          Set @UserID = NULL;  -- do nothing
                                        Else 
                                            Begin
                                          DECLARE @temp As Integer;

                                          SELECT @AlbumID = AlbumID ,
                                                 @Price = Price 
                                            FROM Albums
                                           WHERE AlbumName = '{3}'
                                        ;


                                        if (@Price > @Acct)
	                                        Select -1;
                                        else 
	                                        UPDATE Users
	                                           SET AcctBal = AcctBal - @Price
	                                          WHERE UserID = @UserID
	                                          ; 
	                                        INSERT INTO Purchases(UserID, AmountPaid, PurchaseDate) 
	                                             VALUES(@UserID,@Price,CAST(GETDATE() AS DATE));
	                                        
                                            -- SELECT @MaxPurchase = MAX(PurchaseID) FROM Purchases;
	                                            Set @purchaseID = @@IDENTITY;

                                               INSERT INTO PurchaseDetails(PurchaseID,PurchaseItemTypeID,PurchaseItemID) 
	                                             VALUES(@purchaseID,2,@AlbumID);
                                        End;", textbox2, textbox1, textbox3, listbox1);
                    //MessageBox.Show(sql2);
                    //MessageBox.Show(sql);

                    cmd.CommandText = sql2;
                    int rowsModified = cmd.ExecuteNonQuery();

                    //MessageBox.Show(rowsModified.ToString());

                    if (rowsModified < 3)
                    {
                        this.textBox4.AppendText("Not Enough Balance. Please add value");
                    }
                    else
                    {
                        this.textBox4.AppendText("Item was successfully bought");

                    }

                }

                else
                {
                    //MessageBox.Show("This item has been purchased before");
                    this.textBox4.AppendText("This item has been purchased before");
                }
            }
            
            
            
            db.Close();
        }
    }
}
