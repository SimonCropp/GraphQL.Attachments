import {Component, inject} from '@angular/core';
import {Apollo} from 'apollo-angular';
import {gql} from '@apollo/client/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ClientApp';
  apollo = inject(Apollo);
  selectedFile: File | null = null;
  fileName: string = '';
  uploading: boolean = false;
  uploadMessage: string = '';
  uploadStatus: 'success' | 'error' = 'success';

  // begin-snippet: UseUploadMutation
  uploadFileAsInput = gql`
mutation WithAttachmentAsInput($file: Upload) {
  withAttachmentAsInput(
      argument: "test"
      file:$file
    ){
      argument
    }
}
`

  uploadFile() {
    this.apollo.mutate({
      mutation: this.uploadFileAsInput,
      variables: {file: this.selectedFile},
      context: {
        useMultipart: true  // Important for file uploads with Apollo
      }
    }).subscribe(
      {
        next: ({data}: any) => {
          this.uploading = false;
          if (data) {
            this.uploadMessage = `File ${data.withAttachmentAsInput.argument} uploaded successfully!`;
            this.uploadStatus = 'success';
          }
        },
        error: (error) => {
          this.uploading = false;
          this.uploadMessage = 'Error uploading file: ' + error;
          this.uploadStatus = 'error';
          console.log(error);
        }
      }
    )
  }

// end-snippet

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length) {
      this.selectedFile = input.files[0];
      this.fileName = this.selectedFile.name;
      this.uploadMessage = '';
    }
  }
}

