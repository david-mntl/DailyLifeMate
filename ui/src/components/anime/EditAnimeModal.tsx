import { useEffect } from 'react';
import { useForm, useFieldArray } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useAnime } from '@/hooks/query/useAnime';
import { AnimeDto, UpdateAnimeRequestDto } from '@/types';
import { Plus, X } from 'lucide-react';

interface EditAnimeModalProps {
  isOpen: boolean;
  onClose: () => void;
  contextId: string;
  animeToEdit: AnimeDto | null;
}

const externalLinkSchema = z.object({
  name: z.string().min(1, { message: 'Link name is required.' }),
  url: z.string().url({ message: 'Must be a valid URL.' }),
});

const formSchema = z.object({
  name: z.string().min(1, { message: 'Anime name is required.' }),
  description: z.string().min(1, { message: 'Description is required.' }),
  imageUrl: z.string().url({ message: 'Must be a valid URL.' }).optional().or(z.literal('')),
  externalLinks: z.array(externalLinkSchema).optional(),
});

export function EditAnimeModal({ isOpen, onClose, contextId, animeToEdit }: EditAnimeModalProps) {
  const { updateAnime, isUpdating } = useAnime(contextId);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: '',
      description: '',
      imageUrl: '',
      externalLinks: [],
    },
  });

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: 'externalLinks',
  });

  useEffect(() => {
    if (animeToEdit) {
      form.reset({
        name: animeToEdit.name,
        description: animeToEdit.description,
        imageUrl: animeToEdit.imageUrl || '',
        externalLinks: animeToEdit.externalLinks || [],
      });
    } else {
      form.reset();
    }
  }, [animeToEdit, form]);

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    if (!animeToEdit) return;

    const updatedAnime: UpdateAnimeRequestDto = {
      name: values.name,
      description: values.description,
      externalLinks: values.externalLinks,
    };

    updateAnime(
      { animeId: animeToEdit.id, data: updatedAnime },
      {
        onSuccess: () => {
          form.reset();
          onClose();
        },
      }
    );
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[500px] bg-surface text-text border-border rounded-xl shadow-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-accent">Edit Anime</DialogTitle>
          <DialogDescription className="text-muted-foreground">
            Modify the details for "{animeToEdit?.name || 'selected anime'}".
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4 py-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Name</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Anime Name"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-accent"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="A brief description of the anime."
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-accent resize-y min-h-[80px]"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="imageUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Image URL</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="https://example.com/image.jpg"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-accent"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="space-y-2">
              <FormLabel className="text-text">External Links</FormLabel>
              {fields.map((item, index) => (
                <div key={item.id} className="flex items-end gap-2">
                  <FormField
                    control={form.control}
                    name={`externalLinks.${index}.name`}
                    render={({ field }) => (
                      <FormItem className="flex-1">
                        <FormControl>
                          <Input
                            placeholder="Link Name"
                            {...field}
                            className="bg-background border-border text-text focus-visible:ring-accent"
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name={`externalLinks.${index}.url`}
                    render={({ field }) => (
                      <FormItem className="flex-1">
                        <FormControl>
                          <Input
                            placeholder="https://example.com"
                            {...field}
                            className="bg-background border-border text-text focus-visible:ring-accent"
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    onClick={() => remove(index)}
                    className="text-destructive hover:bg-destructive/20"
                    aria-label="Remove link"
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              ))}
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={() => append({ name: '', url: '' })}
                className="mt-2 w-full border-border text-text hover:bg-surface/50 hover:text-primary transition-colors"
              >
                <Plus className="mr-2 h-4 w-4" /> Add Link
              </Button>
              <FormMessage>{form.formState.errors.externalLinks?.message}</FormMessage>
            </div>

            <DialogFooter className="pt-4">
              <Button
                type="submit"
                disabled={isUpdating}
                className="bg-accent hover:bg-accent/90 text-primary-foreground rounded-lg shadow-md transition-all duration-200 ease-in-out transform hover:scale-105"
              >
                {isUpdating ? 'Saving...' : 'Save Changes'}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
