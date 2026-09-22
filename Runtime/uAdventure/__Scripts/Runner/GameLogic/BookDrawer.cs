using UnityEngine;
using uAdventure.Core;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text;

namespace uAdventure.Runner
{
    public class BookDrawer
    {
        private Texture2D background, leftImage, leftImageOver, rightImage, rightImageOver, closeButton;
        private readonly List<Texture2D> paragraphTextures;

        private readonly ResourceManager resourceManager;
        private readonly GUISkin bookSkin;

        private Rect bookRect = Rect.zero;
        private readonly Rect[] pageRect = new Rect[2];

        private int currentPage = 0;
        private Book book;
        private bool rightIsDefault;

        public Book Book {
            get { return book; }
            set
            {
                if(value != book)
                {
                    currentPage = 0;
                    book = value;
                    RefreshResources();
                }
            }
        }

        public BookDrawer(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
            this.paragraphTextures = new List<Texture2D>();
            this.bookSkin = Resources.Load<GUISkin>("gui/bookskin");
            this.closeButton = Resources.Load<Texture2D>("assets/special/closebook");
        }


        public void Draw(Rect rect)
        {
            if(Book == null || background == null)
            {
                return;
            }

            // Adapt the rect to the background ratio
            var rectRatio = rect.width / rect.height;
            var backgroundRatio = background.width / (float)background.height;
            var rectCenter = rect.center;
            if(rectRatio > backgroundRatio)
            {
                rect.width = rect.height * backgroundRatio;
            }
            else
            {
                rect.height = rect.width / backgroundRatio;
            }
            rect.center = rectCenter;

            // Set matrix
            Vector2 ratio = new Vector2(rect.width / background.width, rect.height / background.height);
            Matrix4x4 guiMatrix = Matrix4x4.TRS(new Vector3(rect.x, rect.y, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));
            rect.position = Vector2.zero;
            rect.size = new Vector2(background.width, background.height);
            GUI.matrix = guiMatrix;


            // First we change the background size to fit the background ratio
            GUI.DrawTexture(rect, background);

            // Draw the content
            var bcSkin = GUI.skin;
            GUI.skin = bookSkin;
            GUILayout.BeginArea(rect, new GUIContent(), "bookBackground");
            bool hasNextPage = false;

            if (Book.getType() == uAdventure.Core.Book.TYPE_PAGES)
            {
                GUILayout.Label("This type of book is not supported in uAdventure.", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            }
            else
            {
                IEnumerator<BookParagraph> paragraphs = Book.getParagraphs().GetEnumerator();
                int lineIndex = 0;
                string[] lines = new string[0];

                GUILayout.BeginHorizontal();
                {
                    float nextElementHeight = 0;
                    GUIStyle nextStyle = new GUIStyle();
                    GUIContent nextContent = new GUIContent();
                    BookParagraph nextElement = null;

                    var drawTillPage = (currentPage + 1) * 2 - 1;
                    // Draw two pages
                    for (int i = 0; i <= drawTillPage; ++i)
                    {
                        bool drawPage = i >= drawTillPage - 1;

                        if (drawPage)
                        {
                            GUILayout.BeginVertical("bookPage", GUILayout.MinWidth(bookRect.width / 2f),
                                GUILayout.Width(bookRect.width / 2f), GUILayout.MaxWidth(bookRect.width / 2f), GUILayout.ExpandHeight(true));
                            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                        }
                        Rect currentPageRect = pageRect[i % 2];
                        float heightRemaining = currentPageRect.height;
                        do
                        {
                            if (nextElement != null)
                            {
                                if (drawPage)
                                {
                                    GUILayout.Label(nextContent, nextStyle, GUILayout.MaxWidth(currentPageRect.width));
                                    if (lineIndex == 0 && nextElement.getType() == BookParagraph.BULLET)
                                    {
                                        GUI.Label(GUILayoutUtility.GetLastRect(), "•", "bullet");
                                    }
                                }
                                heightRemaining -= nextElementHeight;
                                lineIndex++;
                            }

                            if (lineIndex == lines.Length)
                            {
                                if (paragraphs.MoveNext())
                                {
                                    nextElement = paragraphs.Current;
                                    lineIndex = 0;
                                    lines = DivideParagraph(nextElement, currentPageRect.width);
                                    nextStyle = GetElementStyle(nextElement);
                                }
                                else
                                {
                                    nextElement = null;
                                }
                            }

                            if (nextElement != null && lineIndex < lines.Length)
                            {
                                if (nextElement.getType() == BookParagraph.IMAGE)
                                {
                                    var imagePath = nextElement.getContent();
                                    if (!string.IsNullOrEmpty(imagePath))
                                    {
                                        nextContent.text = null;
                                        nextContent.image = resourceManager.getImage(imagePath);
                                    }
                                }
                                else
                                {
                                    nextContent.text = lines[lineIndex];
                                    nextContent.image = null;
                                }

                                nextElementHeight = nextStyle.CalcHeight(nextContent, currentPageRect.width);
                            }

                        } while (nextElementHeight <= heightRemaining && nextElement != null);

                        if (drawPage)
                        {
                            if (nextElement != null)
                            {
                                hasNextPage = i == drawTillPage;
                            }

                            GUILayout.EndVertical();
                            if (Event.current.type == EventType.Repaint)
                            {
                                pageRect[i % 2] = GUILayoutUtility.GetLastRect();
                            }
                            GUILayout.EndVertical();
                        }
                    }
                }
                GUILayout.EndHorizontal();
                if (Event.current.type == EventType.Repaint)
                {
                    bookRect = GUILayoutUtility.GetLastRect();
                }
            }

            GUILayout.EndArea();


            // Reset matrix

            // Lastly we draw the buttons
            if (Application.isPlaying && DoBookButton(rect.position, closeButton, closeButton, rect, false))
            {
                Game.Instance.CloseBook();
            }

            if(this.book != null)
            {
                if (currentPage > 0 && leftImage
                    && DoBookButton(Book.getPreviousPageVector2(), leftImage, leftImageOver, rect, false))
                {
                    currentPage--;
                }

                if (hasNextPage && (rightImage || leftImage)
                    && DoBookButton(Book.getNextPageVector2(), rightImage ?? leftImage, rightImageOver ?? leftImageOver, rect, !rightImage || rightIsDefault))
                {
                    currentPage++;
                }
            }
            GUI.matrix = Matrix4x4.identity;

            GUI.skin = bcSkin;
        }

        private static bool DoBookButton(Vector2 position, Texture2D image, Texture2D imageOver, Rect viewport, bool flip)
        {
            var imageSize = new Vector2(image.width, image.height);
            var buttonRect = new Rect(position, imageSize);
            buttonRect = buttonRect.AdjustToViewport(800, 600, viewport);
            var mouseRect = buttonRect;
            if (flip)
            {
                buttonRect.x += buttonRect.width;
                buttonRect.width = -buttonRect.width;
            }

            var isOver = mouseRect.Contains(Event.current.mousePosition);
            GUI.DrawTexture(buttonRect, isOver ? imageOver : image);

            if(isOver && Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Event.current.Use();
                return true;
            }

            return false;
        }

        private GUIStyle GetElementStyle(BookParagraph nextElement)
        {
            GUIStyle style = new GUIStyle();
            switch (nextElement.getType())
            {
                case BookParagraph.IMAGE:
                    style = GUI.skin.GetStyle("image");
                    break;
                case BookParagraph.BULLET:
                    style = GUI.skin.GetStyle("bookBullet");
                    break;
                case BookParagraph.TEXT:
                    style = GUI.skin.GetStyle("bookText");
                    break;
                case BookParagraph.TITLE:
                    style = GUI.skin.GetStyle("bookTitle");
                    break;
            }
            return style;
        }

        private string[] DivideParagraph(BookParagraph current, float maxWidth)
        {
            var guistyle = GetElementStyle(current);
            var auxContent = new GUIContent { text = " " };
            var oneSpaceWidth = guistyle.CalcSize(auxContent).x;
            auxContent.text = "  ";
            var twoSpaceWidth = guistyle.CalcSize(auxContent).x;
            oneSpaceWidth = twoSpaceWidth - oneSpaceWidth;
            var paddings = twoSpaceWidth - 2 * oneSpaceWidth;
            maxWidth -= paddings;
            List<string> lines = new List<string> ();
            var currentLine = new StringBuilder();

            var paragraphLines = current.getContent().Split('\n');
            if (maxWidth < oneSpaceWidth * 2)
            {
                return paragraphLines.ToList().ConvertAll(_ => "").ToArray();
            }

            foreach (var paragraphLine in paragraphLines)
            {
                var words = paragraphLine.Split(' ');
                float spaceLeft = maxWidth;
                foreach (var word in words)
                {
                    auxContent.text = word;
                    var wordWidth = guistyle.CalcSize(auxContent).x - paddings;
                    if (wordWidth <= spaceLeft)
                    {
                        // Fits in this line
                        if (currentLine.Length > 0)
                        {
                            currentLine.Append(" ");
                            spaceLeft -= oneSpaceWidth;
                        }
                        currentLine.Append(word);
                        spaceLeft -= wordWidth;
                    }
                    else if (wordWidth < maxWidth)
                    {
                        // Fits in next line
                        lines.Add(currentLine.ToString());
                        currentLine.Remove(0, currentLine.Length); // Clear
                        currentLine.Append(word);
                        spaceLeft = maxWidth - wordWidth;
                    }
                    else
                    {
                        // Since it doesn't fit alone, we cut it in pieces.
                        if(currentLine.Length > 0)
                        {
                            lines.Add(currentLine.ToString());
                            currentLine.Remove(0, currentLine.Length); // Clear
                            spaceLeft = maxWidth;
                        }

                        var remaining = word;
                        while (remaining.Length > 0)
                        {
                            auxContent.text = remaining;
                            wordWidth = guistyle.CalcSize(auxContent).x - paddings;

                            if (wordWidth <= maxWidth)
                            {
                                // Base case
                                currentLine.Append(remaining);
                                remaining = string.Empty;
                                spaceLeft -= wordWidth;
                            }
                            else
                            {
                                // Recursive case
                                var percent = maxWidth / wordWidth;
                                var index = (int)percent * remaining.Length;

                                var lastValidIndex = -1;
                                do
                                {
                                    auxContent.text = remaining.Substring(0, index);
                                    var selectedWidth = guistyle.CalcSize(auxContent).x - paddings;
                                    if (selectedWidth <= maxWidth)
                                    {
                                        lastValidIndex = index;
                                        index++;
                                        if (remaining.Length == index)
                                        {
                                            // We cant move forward, so this is valid already
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        index--;
                                    }
                                } while (lastValidIndex != index);

                                lines.Add(remaining.Substring(0, index));
                                remaining = remaining.Substring(index);
                                spaceLeft = maxWidth;
                            }
                        }
                    }
                }
                lines.Add(currentLine.ToString());
                currentLine.Remove(0, currentLine.Length); // Clear
            }
            
            return lines.ToArray();
        }

        public void RefreshResources()
        {
            if(Book == null)
            {
                return;
            }

            var bookResources = Book.getResources().Checked().FirstOrDefault();
            if (bookResources != null)
            {
                background = LoadResource(bookResources, Book.RESOURCE_TYPE_BACKGROUND);
                leftImage = LoadResource(bookResources, Book.RESOURCE_TYPE_ARROW_LEFT_NORMAL);
                if (!leftImage)
                {
                    leftImage = resourceManager.getImage(SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL);
                }
                leftImageOver = LoadResource(bookResources, Book.RESOURCE_TYPE_ARROW_LEFT_OVER);
                if (!leftImageOver)
                {
                    leftImageOver = resourceManager.getImage(SpecialAssetPaths.ASSET_DEFAULT_ARROW_OVER);
                }
                rightIsDefault = bookResources.getAssetPath(Book.RESOURCE_TYPE_ARROW_RIGHT_NORMAL) == SpecialAssetPaths.ASSET_DEFAULT_ARROW_NORMAL;

                rightImage = LoadResource(bookResources, Book.RESOURCE_TYPE_ARROW_RIGHT_NORMAL);
                rightImageOver = LoadResource(bookResources, Book.RESOURCE_TYPE_ARROW_RIGHT_OVER);
            }

            foreach (var paragraph in Book.getParagraphs())
            {
                if (paragraph.getType() == BookParagraph.IMAGE)
                {
                    paragraphTextures.Add(resourceManager.getImage(paragraph.getContent()));
                }
            }
        }

        private Texture2D LoadResource(ResourcesUni resources, string uri)
        {
            Texture2D image = null;
            var path = resources.getAssetPath(uri);
            if (!string.IsNullOrEmpty(path))
            {
                image = resourceManager.getImage(path);
            }
            return image;
        }
    }
}
