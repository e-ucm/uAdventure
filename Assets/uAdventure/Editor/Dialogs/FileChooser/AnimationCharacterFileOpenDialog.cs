namespace uAdventure.Editor
{
    public class AnimationCharacterFileOpenDialog : AnimationFileOpenDialog
    {

        private CharactersWindowAppearance.CharacterAnimationType animType;
        public void Init(DialogReceiverInterface e, FileType fType, CharactersWindowAppearance.CharacterAnimationType type)
        {
            animType = type;
            base.Init(e, fType);
        }

        protected override void ChoosedCorrectFile()
        {
            CopySelectedAssset();
            reference.OnDialogOk(returnPath, fileType, animType);
        }
    }
}