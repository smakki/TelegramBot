import React from 'react';
import { Editor } from "@tinymce/tinymce-react";
import '../index.css';
interface TinyMCEEditorProps {
    apiKey: string;
    initialValue: string;
    init: {
        height: number;
        menubar: boolean;
        plugins: string[];
        toolbar: string;
    }
    onInit?: (evt: any, editor: any) => void;
}
const mceEditor: React.FC<TinyMCEEditorProps> = ({ apiKey, initialValue, init, onInit }) =>
{
    return <Editor apiKey={apiKey} initialValue={initialValue} init={init} onInit={onInit}/>;
};
export default mceEditor;
