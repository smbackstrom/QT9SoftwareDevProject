<%@ Page Title="Movies" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Movies.WebForms._Default" Async="true" %>

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
                        CssClass="error-message">
                    </asp:Label>

                   <div class="movie-toolbar">
                    <span class="toggle-text">Show Inactive</span>

                    <label class="switch">
                        <input
                            id="chkShowInactive"
                            runat="server"
                            type="checkbox"
                            onchange="this.form.submit();"
                            onserverchange="chkShowInactive_ServerChange" />

                        <span class="slider"></span>
                    </label>
                </div>

                    <asp:GridView 
                        ID="MovieGrid" 
                        runat="server" 
                        AutoGenerateColumns="False"                         
                        CssClass="movie-grid"
                        GridLines="Both"
                        EmptyDataText="No movies were returned."
                        OnRowCommand="MovieGrid_RowCommandAsync">

                        <RowStyle CssClass="movie-row" />
                        <AlternatingRowStyle CssClass="movie-row-alt" />

                        <Columns>
                            
                            <asp:BoundField DataField="MovieID" HeaderText="Movie ID" />

                            <asp:BoundField DataField="MovieTitle" HeaderText="Title" />

                            <asp:BoundField DataField="MovieRating" HeaderText="Rating" />

                            <asp:BoundField DataField="ReleaseYear" HeaderText="Release Year" />

                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="EditButton"
                                        runat="server"
                                         Text="<i class='bi bi-pencil'></i>"
                                        ToolTip="Edit Movie"
                                        CommandName="EditMovie"
                                        CommandArgument='<%# Eval("MovieID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:LinkButton
                                        ID="DeleteButton"
                                        runat="server"
                                        Text="<i class='bi bi-trash2'></i>"
                                        ToolTip="Delete Movie"
                                        CommandName="DeleteMovie"
                                        CommandArgument='<%# Eval("MovieID") %>'
                                        OnClientClick="return confirm('Are you sure you want to delete this movie?');" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView> 
                </p>
            <%--<h6>Selected Movie</h6>
            <asp:TextBox
                ID="SelectedMovieTextBox"
                runat="server"
                TextMode="MultiLine"
                Rows="5"
                ReadOnly="true"
                CssClass="selected-movie" />--%>
        </div>
    </main>

</asp:Content>
