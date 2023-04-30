using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class EmojisItemRequest
    {
        public string Name
        {
            get; set;
        } = string.Empty;
        public string Desc
        {
            get;
            set;
        } = string.Empty;
        public string VideoFileId
        {
            get; set;
        } = string.Empty;
        public string PictureFileId 
        { 
            get; set; 
        } = string.Empty;
        public string PictureFileName 
        { 
            get; set; 
        } = string.Empty;

        public string Author
        {
            get;
            set;
        } = string.Empty;
    }

}
