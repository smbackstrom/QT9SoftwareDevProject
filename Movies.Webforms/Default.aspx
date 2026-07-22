<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Movies.WebForms._Default" Async="true" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <h1 id="aspnetTitle">Movies</h1>
           
        </section>

        <div class="row"> 
                <p>
                    <asp:Label 
                        ID="ErrorLabel" 
                        runat="server"
                        CssClass="error-message"
                        ForeColor="Red">
                    </asp:Label>
                    <asp:GridView 
                        ID="MovieGrid" 
                        runat="server" 
                        AutoGenerateColumns="False"                         
                        CssClass="movie-grid"
                        GridLines="Both"
                        EmptyDataText="No movies were returned."
                        OnRowCommand="MovieGrid_RowCommand">

                        <RowStyle CssClass="movie-row" />
                        <AlternatingRowStyle CssClass="movie-row-alt" />

                        <Columns>
                            <asp:TemplateField HeaderText="Display below">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="SelectMovieLinkButton"
                                        runat="server"
                                        Text="Select"
                                        CommandName="SelectedMovie"
                                        CommandArgument="<%# Container.DataItemIndex %>"
                                        CausesValidation="false" />
                                </ItemTemplate>
                            </asp:TemplateField>  
                            
                            <asp:BoundField DataField="MovieID" HeaderText="Movie ID" />

                            <asp:BoundField DataField="MovieTitle" HeaderText="Title" />

                            <asp:BoundField DataField="MovieRating" HeaderText="Rating" />

                            <asp:BoundField DataField="ReleaseYear" HeaderText="Release Year" />
                        </Columns>

                    </asp:GridView> 
                </p>
               <!--<h2>Selected Movie</h2> -->

            <asp:TextBox
                ID="SelectedMovieTextBox"
                runat="server"
                TextMode="MultiLine"
                Rows="5"
                ReadOnly="true"
                CssClass="selected-movie" />
        </div>
    </main>

</asp:Content>
