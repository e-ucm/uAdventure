public enum ConversationNodeViewEnum { 
    /**
     * Constant for dialogue node.
     */
    DIALOGUE = 0,
    /**
     * Constant for option node.
     */
     OPTION = 1
 };
public interface ConversationNodeView
{

    int getEditorX();

    void setEditorX(int xEditor);

    int getEditorY();

    void setEditorY(int yEditor);


    /**
     * Returns the type of the current node.
     * 
     * @return DIALOGUE if dialogue node, OPTION if option node
     */
    ConversationNodeViewEnum getType();

    /**
     * Returns if the node is terminal (has no children).
     * 
     * @return True if the node is terminal, false otherwise
     */
    bool isTerminal();

    /**
     * Returns the children's number of the node.
     * 
     * @return The number of children
     */
    int getChildCount();

    /**
     * Returns the view conversation node in the given position.
     * 
     * @param index
     *            Index of the child
     * @return Selected reduced child
     */
    ConversationNodeView getChildView(int index);

    /**
     * Returns the lines' number of the node.
     * 
     * @return The number of lines
     */
    int getLineCount();

    /**
     * Returns whether the given line belongs to the player or not.
     * 
     * @param index
     *            Index of the line
     * @return True if the line belongs to the player, false otherwise
     */
    bool isPlayerLine(int index);

    /**
     * Returns the name of the line in the given index.
     * 
     * @param index
     *            Index of the line
     * @return Name of the line
     */
    string getLineName(int index);

    /**
     * Returns the line in the specified position.
     * 
     * @param index
     *            Index for extraction
     * @return The conversation line selected
     */
    ConversationLine getLine(int index);

    /**
     * Returns the text of the line in the given index.
     * 
     * @param index
     *            Index of the line
     * @return Text of the line
     */
    string getLineText(int index);

    /**
     * Returns the path of the audio for the given index.
     * 
     * @param index
     *            Index of the line
     * @return Text of the line
     */
    string getAudioPath(int index);

    /**
     * Checks whether the line for the given index has a valid audio path
     * 
     * @param index
     *            Index of the line
     * 
     * @return True if has audio path, false otherwise
     */
    bool hasAudioPath(int index);

    /**
     * Returns if the node has a valid effect set.
     * 
     * @return True if the node has a set of effects, false otherwise
     */
    bool hasEffects();

    /**
     * Returns the conditions of the line in the given index.
     * 
     * @param index
     *            Index of the line
     * @return Conditions of the line
     */
    Conditions getLineConditions(int index);

    /**
     * Returns the conversation line in the given index.
     * 
     * @param index
     *            Index of the line
     * @return Conversation Line
     */
    ConversationLine getConversationLine(int index);
}