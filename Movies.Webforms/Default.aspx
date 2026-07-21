<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Movies.Webforms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle">Movies</h1>
           
        </section>

        <div class="row"> 
            <section class="col-md-4" aria-labelledby="hostingTitle">
                <p>
                    <asp:Label ID="ErrorLabel" runat="server"></asp:Label>
                    <asp:GridView 
                        ID="MovieGrid" 
                        runat="server" 
                        AutoGenerateColumns="false" 
                        GridLines="Both" 
                        EmptyDataText="No movies were returned.">

                        <Columns>

                        </Columns>


                    </asp:GridView> 
                </p>
               
            </section>
        </div>
    </main>

</asp:Content>
